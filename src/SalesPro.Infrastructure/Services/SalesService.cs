using Microsoft.EntityFrameworkCore;
using SalesPro.Application.Dtos;
using SalesPro.Application.Exceptions;
using SalesPro.Application.Services;
using SalesPro.Domain.Entities;
using SalesPro.Infrastructure.Persistence;

namespace SalesPro.Infrastructure.Services;

public sealed class SalesService : ISalesService
{
    private readonly SalesDbContext _db;

    public SalesService(SalesDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateSaleAsync(SaleCreateDto dto, CancellationToken ct = default)
    {
        if (dto.CustomerId <= 0) throw new BusinessRuleException("Selecciona un cliente válido.");
        if (dto.Items is null || dto.Items.Count == 0) throw new BusinessRuleException("Agrega al menos un producto.");
        if (dto.VatRate < 0 || dto.VatRate > 1) throw new BusinessRuleException("IVA inválido.");

        var distinct = dto.Items.Select(x => x.ProductId).Distinct().Count();
        if (distinct != dto.Items.Count) throw new BusinessRuleException("No se permiten productos repetidos en la misma venta.");

        foreach (var it in dto.Items)
        {
            if (it.ProductId <= 0) throw new BusinessRuleException("Producto inválido.");
            if (it.Quantity <= 0) throw new BusinessRuleException("Cantidad inválida.");
        }

        var customerExists = await _db.Customers.AnyAsync(c => c.CustomerId == dto.CustomerId && c.IsActive, ct);
        if (!customerExists) throw new BusinessRuleException("El cliente seleccionado no existe o está inactivo.");

        var orderedItems = dto.Items.OrderBy(x => x.ProductId).ToList();

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var sale = new Sale
        {
            CustomerId = dto.CustomerId,
            SaleDate = DateTime.UtcNow,
            VatRate = dto.VatRate,
            InvoiceNumber = "PENDIENTE",
            Subtotal = 0,
            VatAmount = 0,
            Total = 0
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync(ct);

        var saleItems = new List<SaleItem>(orderedItems.Count);

        decimal subtotal = 0m;

        foreach (var it in orderedItems)
        {
            var product = await _db.Products
                .FromSqlInterpolated($"SELECT * FROM Productos WHERE producto_id = {it.ProductId} FOR UPDATE")
                .AsTracking()
                .SingleOrDefaultAsync(ct);

            if (product is null || !product.IsActive)
                throw new BusinessRuleException("Uno de los productos no existe o está inactivo.");

            if (product.Stock < it.Quantity)
                throw new BusinessRuleException($"Stock insuficiente para {product.Name}. Disponible: {product.Stock}.");

            var unitPrice = product.Price;
            var lineSubtotal = RoundMoney(unitPrice * it.Quantity);

            product.Stock -= it.Quantity;

            saleItems.Add(new SaleItem
            {
                SaleId = sale.SaleId,
                ProductId = product.ProductId,
                Quantity = it.Quantity,
                UnitPrice = unitPrice,
                LineSubtotal = lineSubtotal
            });

            subtotal += lineSubtotal;
        }

        subtotal = RoundMoney(subtotal);
        var vatAmount = RoundMoney(subtotal * dto.VatRate);
        var total = RoundMoney(subtotal + vatAmount);

        sale.Subtotal = subtotal;
        sale.VatAmount = vatAmount;
        sale.Total = total;

        sale.InvoiceNumber = BuildInvoiceNumber(sale.SaleId, sale.SaleDate);

        _db.SaleItems.AddRange(saleItems);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return sale.SaleId;
    }

    public async Task<SaleDetailsDto?> GetSaleDetailsAsync(int saleId, CancellationToken ct = default)
    {
        var sale = await _db.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .SingleOrDefaultAsync(s => s.SaleId == saleId, ct);

        if (sale is null) return null;

        return new SaleDetailsDto
        {
            SaleId = sale.SaleId,
            InvoiceNumber = sale.InvoiceNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerCedula = sale.Customer.Cedula,
            CustomerFullName = $"{sale.Customer.FirstName} {sale.Customer.LastName}",
            CustomerEmail = sale.Customer.Email,
            CustomerPhone = sale.Customer.Phone,
            Subtotal = sale.Subtotal,
            VatRate = sale.VatRate,
            VatAmount = sale.VatAmount,
            Total = sale.Total,
            Items = sale.Items
                .OrderBy(i => i.SaleItemId)
                .Select(i => new SaleDetailsItemDto
                {
                    ProductId = i.ProductId,
                    ProductCode = i.Product.Code,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineSubtotal = i.LineSubtotal
                }).ToList()
        };
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string BuildInvoiceNumber(int id, DateTime dateUtc)
    {
        var datePart = dateUtc.ToString("yyyyMMdd");
        return $"V-{datePart}-{id:000000}";
    }
}
