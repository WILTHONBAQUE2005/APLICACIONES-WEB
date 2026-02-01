
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models;

namespace SistemaPedidos.Controllers;

public class CustomersController : Controller
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, bool showInactive = false)
    {
        var query = _db.Customers.AsNoTracking();

        if (!showInactive)
            query = query.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(c =>
                c.DocumentNumber.Contains(q) ||
                c.FirstName.Contains(q) ||
                c.LastName.Contains(q) ||
                c.Email.Contains(q));
        }

        var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _db.Customers
            .AsNoTracking()
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    public IActionResult Create()
    {
        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.CreatedAt = DateTime.UtcNow;
        model.IsActive = true;

        _db.Customers.Add(model);

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Cliente creado.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. Revisa si la cédula o email ya existen.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.CustomerId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var entity = await _db.Customers.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.DocumentNumber = model.DocumentNumber.Trim();
        entity.FirstName = model.FirstName.Trim();
        entity.LastName = model.LastName.Trim();
        entity.Email = model.Email.Trim();
        entity.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
        entity.IsActive = model.IsActive;

        try
        {
            await _db.SaveChangesAsync();
            TempData["ok"] = "Cliente actualizado.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar. Revisa si la cédula o email ya existen.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        customer.IsActive = !customer.IsActive;
        await _db.SaveChangesAsync();

        TempData["ok"] = customer.IsActive ? "Cliente activado." : "Cliente desactivado.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
