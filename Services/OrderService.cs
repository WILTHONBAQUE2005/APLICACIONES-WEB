
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models;
using SistemaPedidos.Models.Enums;
using SistemaPedidos.ViewModels;

namespace SistemaPedidos.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(OrderCreateVm input)
    {
        if (input.Items == null || input.Items.Count == 0)
            throw new InvalidOperationException("El pedido debe tener al menos un producto.");

        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == input.CustomerId && c.IsActive);
        if (customer == null)
            throw new InvalidOperationException("Cliente no válido.");

        var productIds = input.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.ProductId) && p.IsActive).ToListAsync();

        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Uno o más productos no son válidos.");

        foreach (var row in input.Items)
        {
            var p = products.First(x => x.ProductId == row.ProductId);
            if (row.Quantity <= 0)
                throw new InvalidOperationException("Cantidad inválida.");

            if (p.Stock < row.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para {p.Name}. Disponible: {p.Stock}.");
        }

        using var tx = await _db.Database.BeginTransactionAsync();

        var today = DateTime.UtcNow.Date;
        var todayCount = await _db.Orders.CountAsync(o => o.CreatedAt >= today && o.CreatedAt < today.AddDays(1));
        var orderNumber = $"P-{DateTime.UtcNow:yyyyMMdd}-{(todayCount + 1).ToString("D4")}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = customer.CustomerId,
            OrderDate = input.OrderDate,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        decimal subtotal = 0m;

        foreach (var row in input.Items)
        {
            var p = products.First(x => x.ProductId == row.ProductId);
            var unitPrice = p.UnitPrice;
            var lineTotal = Math.Round(unitPrice * row.Quantity, 2, MidpointRounding.AwayFromZero);

            order.Items.Add(new OrderItem
            {
                ProductId = p.ProductId,
                Quantity = row.Quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            });

            subtotal += lineTotal;

            p.Stock -= row.Quantity;

            _db.InventoryMovements.Add(new InventoryMovement
            {
                ProductId = p.ProductId,
                Type = InventoryMovementType.Out,
                Quantity = row.Quantity,
                Reason = "Venta",
                Reference = orderNumber,
                CreatedAt = DateTime.UtcNow
            });
        }

        order.Subtotal = Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);
        order.Discount = Math.Round(Math.Max(0m, input.Discount), 2, MidpointRounding.AwayFromZero);
        order.Tax = 0m;
        order.Total = Math.Round(Math.Max(0m, order.Subtotal - order.Discount + order.Tax), 2, MidpointRounding.AwayFromZero);

        _db.Orders.Add(order);

        _db.Shipments.Add(new Shipment
        {
            Order = order,
            Status = ShipmentStatus.Pending,
            ShipTo = customer.FullName
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return order.OrderId;
    }

    public async Task AddPaymentAsync(PaymentCreateVm input)
    {
        var order = await _db.Orders.Include(o => o.Payments).FirstOrDefaultAsync(o => o.OrderId == input.OrderId);
        if (order == null)
            throw new InvalidOperationException("Pedido no encontrado.");

        if (order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("No se puede pagar un pedido cancelado.");

        var payment = new Payment
        {
            OrderId = order.OrderId,
            Method = input.Method,
            Amount = Math.Round(input.Amount, 2, MidpointRounding.AwayFromZero),
            PaidAt = DateTime.UtcNow,
            Reference = string.IsNullOrWhiteSpace(input.Reference) ? null : input.Reference.Trim()
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        await RecalculatePaymentStatusAsync(order.OrderId);
    }

    public async Task RecalculatePaymentStatusAsync(int orderId)
    {
        var order = await _db.Orders.Include(o => o.Payments).FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
            return;

        var paid = order.Payments.Sum(p => p.Amount);

        if (paid <= 0m)
            order.PaymentStatus = PaymentStatus.Unpaid;
        else if (paid + 0.001m < order.Total)
            order.PaymentStatus = PaymentStatus.Partial;
        else
            order.PaymentStatus = PaymentStatus.Paid;

        if (order.PaymentStatus == PaymentStatus.Paid && order.Status == OrderStatus.Pending)
            order.Status = OrderStatus.Paid;

        await _db.SaveChangesAsync();
    }

    public async Task UpdateShipmentAsync(ShipmentUpdateVm input)
    {
        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.OrderId == input.OrderId);
        if (shipment == null)
            throw new InvalidOperationException("Envío no encontrado.");

        shipment.Carrier = string.IsNullOrWhiteSpace(input.Carrier) ? null : input.Carrier.Trim();
        shipment.TrackingNumber = string.IsNullOrWhiteSpace(input.TrackingNumber) ? null : input.TrackingNumber.Trim();
        shipment.ShipTo = string.IsNullOrWhiteSpace(input.ShipTo) ? null : input.ShipTo.Trim();
        shipment.Status = input.Status;

        if (input.Status == ShipmentStatus.Shipped && shipment.ShippedAt == null)
            shipment.ShippedAt = DateTime.UtcNow;

        if (input.Status == ShipmentStatus.Delivered && shipment.DeliveredAt == null)
            shipment.DeliveredAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
            throw new InvalidOperationException("Pedido no encontrado.");

        order.Status = status;
        await _db.SaveChangesAsync();
    }
}
