
using Microsoft.AspNetCore.Mvc;
using SistemaPedidos.Services;
using SistemaPedidos.ViewModels;

namespace SistemaPedidos.Controllers;

public class HomeController : Controller
{
    private readonly DashboardService _dashboard;

    public HomeController(DashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    public async Task<IActionResult> Index()
    {
        DashboardVm vm = await _dashboard.GetAsync();
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
