
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models;
using SistemaPedidos.Services;

namespace SistemaPedidos.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private readonly InventoryService _inventory;

    public ProductsController(AppDbContext db, InventoryService inventory)
    {
        _db = db;
        _inventory = inventory;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId, bool showInactive = false)
    {
        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!showInactive)
            query = query.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(p => p.Sku.Contains(q) || p.Name.Contains(q));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", categoryId);

        var list = await query.OrderBy(p => p.Name).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        var moves = await _db.InventoryMovements
            .AsNoTracking()
            .Where(m => m.ProductId == id)
            .OrderByDescending(m => m.CreatedAt)
            .Take(25)
            .ToListAsync();

        ViewBag.Movements = moves;
        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name");
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", model.CategoryId);
            return View(model);
        }

        model.Sku = model.Sku.Trim();
        model.Name = model.Name.Trim();
        model.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        model.CreatedAt = DateTime.UtcNow;
        model.IsActive = true;

        _db.Products.Add(model);

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Producto creado.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. Verifica SKU único y categoría.");
            ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", model.CategoryId);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model)
    {
        if (id != model.ProductId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", model.CategoryId);
            return View(model);
        }

        var entity = await _db.Products.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.Sku = model.Sku.Trim();
        entity.Name = model.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        entity.UnitPrice = model.UnitPrice;
        entity.Stock = model.Stock;
        entity.CategoryId = model.CategoryId;
        entity.IsActive = model.IsActive;

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Producto actualizado.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. Verifica SKU único y categoría.");
            ViewBag.Categories = new SelectList(await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(), "CategoryId", "Name", model.CategoryId);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(int id, int quantity, string? reason)
    {
        await _inventory.AdjustAsync(id, quantity, reason);
        TempData["ok"] = "Stock ajustado.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
