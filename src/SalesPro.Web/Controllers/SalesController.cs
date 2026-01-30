using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalesPro.Application.Dtos;
using SalesPro.Application.Exceptions;
using SalesPro.Application.Services;
using SalesPro.Infrastructure.Persistence;
using SalesPro.Web.ViewModels;

namespace SalesPro.Web.Controllers;

public sealed class SalesController : Controller
{
    private readonly SalesDbContext _db;
    private readonly ISalesService _sales;
    private readonly SalesOptions _options;

    public SalesController(SalesDbContext db, ISalesService sales, IOptions<SalesOptions> options)
    {
        _db = db;
        _sales = sales;
        _options = options.Value;
    }

    public async Task<IActionResult> Index(string? q, int p = 1, int pageSize = 10, CancellationToken ct = default)
    {
        p = Math.Max(1, p);
        pageSize = Math.Clamp(pageSize, 5, 50);

        IQueryable<SalesPro.Domain.Entities.Sale> query = _db.Sales.AsNoTracking().Include(s => s.Customer);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(s =>
                s.InvoiceNumber.Contains(term) ||
                s.Customer.Cedula.Contains(term) ||
                (s.Customer.FirstName + " " + s.Customer.LastName).Contains(term));
        }

        var total = await query.CountAsync(ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        if (p > totalPages) p = totalPages;

        var items = await query
            .OrderByDescending(s => s.SaleId)
            .Skip((p - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SaleListRowVm
            {
                SaleId = s.SaleId,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                Customer = s.Customer.FirstName + " " + s.Customer.LastName,
                Cedula = s.Customer.Cedula,
                Total = s.Total
            })
            .ToListAsync(ct);

        var baseUrl = Url.Action("Index", "Sales", new { q, pageSize }) ?? "/Sales";
        var vm = new PagedListVm<SaleListRowVm>
        {
            Items = items,
            Query = q,
            Pager = new PagerVm
            {
                CurrentPage = p,
                TotalPages = totalPages,
                PageSize = pageSize,
                BaseUrl = baseUrl.Contains("?") ? baseUrl : baseUrl + "?"
            }
        };

        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new SaleCreateVm { VatRate = _options.DefaultVatRate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCreateVm vm, CancellationToken ct)
    {
        if (vm.Items is null) vm.Items = new List<SaleItemVm>();

        var duplicates = vm.Items
            .GroupBy(i => i.ProductId)
            .Where(g => g.Key > 0 && g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
        {
            ModelState.AddModelError("", "No se permiten productos repetidos en la misma venta.");
        }

        if (!ModelState.IsValid)
        {
            if (vm.VatRate <= 0) vm.VatRate = _options.DefaultVatRate;
            TempData["ToastError"] = "Revisa los datos de la venta.";
            return View(vm);
        }

        try
        {
            var dto = new SaleCreateDto
            {
                CustomerId = vm.CustomerId,
                VatRate = vm.VatRate,
                Items = vm.Items.Select(i => new SaleItemCreateDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var saleId = await _sales.CreateSaleAsync(dto, ct);
            TempData["Toast"] = "Venta registrada.";
            return RedirectToAction(nameof(Details), new { id = saleId });
        }
        catch (BusinessRuleException ex)
        {
            ModelState.AddModelError("", ex.Message);
            TempData["ToastError"] = ex.Message;
            if (vm.VatRate <= 0) vm.VatRate = _options.DefaultVatRate;
            return View(vm);
        }
        catch (DbUpdateException)
        {
            TempData["ToastError"] = "No se pudo guardar la venta. Verifica los datos e intenta nuevamente.";
            if (vm.VatRate <= 0) vm.VatRate = _options.DefaultVatRate;
            return View(vm);
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _sales.GetSaleDetailsAsync(id, ct);
        if (dto is null) return NotFound();
        return View(dto);
    }
}

public sealed class SaleListRowVm
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime SaleDate { get; set; }
    public string Customer { get; set; } = "";
    public string Cedula { get; set; } = "";
    public decimal Total { get; set; }
}
