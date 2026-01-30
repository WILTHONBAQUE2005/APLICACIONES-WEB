using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPro.Infrastructure.Persistence;

namespace SalesPro.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly SalesDbContext _db;

    public HomeController(SalesDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await _db.Products.CountAsync(ct);
        var customers = await _db.Customers.CountAsync(ct);
        var sales = await _db.Sales.CountAsync(ct);
        var totalSales = await _db.Sales.SumAsync(s => (decimal?)s.Total, ct) ?? 0m;

        ViewBag.Products = products;
        ViewBag.Customers = customers;
        ViewBag.Sales = sales;
        ViewBag.TotalSales = totalSales;

        return View();
    }

    public IActionResult Error()
    {
        return View();
    }

    public IActionResult Status(int code)
    {
        ViewBag.Code = code;
        return View();
    }
}
