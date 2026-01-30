using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPro.Domain.Entities;
using SalesPro.Infrastructure.Persistence;
using SalesPro.Web.ViewModels;

namespace SalesPro.Web.Controllers;

public sealed class ProductsController : Controller
{
    private readonly SalesDbContext _db;

    public ProductsController(SalesDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int p = 1, int pageSize = 10, CancellationToken ct = default)
    {
        p = Math.Max(1, p);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(x =>
                x.Code.Contains(term) ||
                x.Name.Contains(term));
        }

        var total = await query.CountAsync(ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

        if (p > totalPages) p = totalPages;

        var items = await query
            .OrderByDescending(x => x.ProductId)
            .Skip((p - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductListRowVm
            {
                ProductId = x.ProductId,
                Code = x.Code,
                Name = x.Name,
                Price = x.Price,
                Stock = x.Stock,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);

        var baseUrl = Url.Action("Index", "Products", new { q, pageSize }) ?? "/Products";
        var vm = new PagedListVm<ProductListRowVm>
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
        return View(new ProductUpsertVm { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductUpsertVm vm, CancellationToken ct)
    {
        if (vm.Price < 0.01m) ModelState.AddModelError(nameof(vm.Price), "El precio debe ser mayor o igual a 0.01.");
        if (vm.Stock < 0) ModelState.AddModelError(nameof(vm.Stock), "El stock no puede ser negativo.");

        if (!ModelState.IsValid) return View(vm);

        var code = vm.Code.Trim().ToUpperInvariant();

        var exists = await _db.Products.AnyAsync(x => x.Code == code, ct);
        if (exists)
        {
            ModelState.AddModelError(nameof(vm.Code), "El código ya existe.");
            return View(vm);
        }

        var entity = new Product
        {
            Code = code,
            Name = vm.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim(),
            Price = vm.Price,
            Stock = vm.Stock,
            IsActive = vm.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Producto creado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var p = await _db.Products.SingleOrDefaultAsync(x => x.ProductId == id, ct);
        if (p is null) return NotFound();

        return View(new ProductUpsertVm
        {
            ProductId = p.ProductId,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            IsActive = p.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductUpsertVm vm, CancellationToken ct)
    {
        if (vm.Price < 0.01m) ModelState.AddModelError(nameof(vm.Price), "El precio debe ser mayor o igual a 0.01.");
        if (vm.Stock < 0) ModelState.AddModelError(nameof(vm.Stock), "El stock no puede ser negativo.");

        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.Products.SingleOrDefaultAsync(x => x.ProductId == vm.ProductId, ct);
        if (entity is null) return NotFound();

        var code = vm.Code.Trim().ToUpperInvariant();
        var exists = await _db.Products.AnyAsync(x => x.ProductId != vm.ProductId && x.Code == code, ct);
        if (exists)
        {
            ModelState.AddModelError(nameof(vm.Code), "El código ya existe.");
            return View(vm);
        }

        entity.Code = code;
        entity.Name = vm.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim();
        entity.Price = vm.Price;
        entity.Stock = vm.Stock;
        entity.IsActive = vm.IsActive;

        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Producto actualizado.";
        return RedirectToAction(nameof(Index), new { q = vm.Code });
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var p = await _db.Products.AsNoTracking().SingleOrDefaultAsync(x => x.ProductId == id, ct);
        if (p is null) return NotFound();
        return View(p);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var p = await _db.Products.AsNoTracking().SingleOrDefaultAsync(x => x.ProductId == id, ct);
        if (p is null) return NotFound();
        return View(p);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var p = await _db.Products.SingleOrDefaultAsync(x => x.ProductId == id, ct);
        if (p is null) return NotFound();

        var used = await _db.SaleItems.AnyAsync(x => x.ProductId == id, ct);
        if (used)
        {
            TempData["ToastError"] = "No se puede eliminar: el producto ya tiene ventas asociadas.";
            return RedirectToAction(nameof(Index));
        }

        _db.Products.Remove(p);
        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Producto eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string q, CancellationToken ct)
    {
        var term = (q ?? "").Trim();
        if (term.Length < 2) return Json(Array.Empty<object>());

        var list = await _db.Products
            .AsNoTracking()
            .Where(x => x.IsActive && (x.Code.Contains(term) || x.Name.Contains(term)))
            .OrderBy(x => x.Code)
            .Take(10)
            .Select(x => new
            {
                id = x.ProductId,
                code = x.Code,
                name = x.Name,
                price = x.Price,
                stock = x.Stock
            })
            .ToListAsync(ct);

        return Json(list);
    }
}

public sealed class ProductListRowVm
{
    public int ProductId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
}
