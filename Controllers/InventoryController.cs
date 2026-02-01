
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;

namespace SistemaPedidos.Controllers;

public class InventoryController : Controller
{
    private readonly AppDbContext _db;

    public InventoryController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var lowStock = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Stock <= 5)
            .OrderBy(p => p.Stock)
            .ThenBy(p => p.Name)
            .ToListAsync();

        var moves = await _db.InventoryMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .OrderByDescending(m => m.CreatedAt)
            .Take(80)
            .ToListAsync();

        ViewBag.LowStock = lowStock;
        return View(moves);
    }
}
