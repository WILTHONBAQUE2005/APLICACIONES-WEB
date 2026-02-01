
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models;

namespace SistemaPedidos.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound();

        return View(category);
    }

    public IActionResult Create()
    {
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.CreatedAt = DateTime.UtcNow;
        _db.Categories.Add(model);

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Categoría creada.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. El nombre ya existe.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.CategoryId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var entity = await _db.Categories.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.Name = model.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        entity.IsActive = model.IsActive;

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Categoría actualizada.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. El nombre ya existe.");
            return View(model);
        }
    }
}
