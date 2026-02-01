
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Controllers;

public class ReportsController : Controller
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Sales(DateTime? from, DateTime? to)
    {
        var start = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var end = to ?? DateTime.UtcNow.Date.AddDays(1);

        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Where(o => o.OrderDate >= start && o.OrderDate < end && o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Draft)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        ViewBag.From = start;
        ViewBag.To = end.AddDays(-1);
        return View(orders);
    }
}
