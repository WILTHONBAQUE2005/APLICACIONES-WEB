using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPro.Domain.Entities;
using SalesPro.Infrastructure.Persistence;
using SalesPro.Web.ViewModels;

namespace SalesPro.Web.Controllers;

public sealed class CustomersController : Controller
{
    private readonly SalesDbContext _db;

    public CustomersController(SalesDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int p = 1, int pageSize = 10, CancellationToken ct = default)
    {
        p = Math.Max(1, p);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            if (term.All(char.IsDigit))
            {
                query = query.Where(x => x.Cedula.Contains(term));
            }
            else
            {
                query = query.Where(x =>
                    x.FirstName.Contains(term) ||
                    x.LastName.Contains(term) ||
                    (x.FirstName + " " + x.LastName).Contains(term));
            }
        }

        var total = await query.CountAsync(ct);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        if (p > totalPages) p = totalPages;

        var items = await query
            .OrderByDescending(x => x.CustomerId)
            .Skip((p - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerListRowVm
            {
                CustomerId = x.CustomerId,
                Cedula = x.Cedula,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);

        var baseUrl = Url.Action("Index", "Customers", new { q, pageSize }) ?? "/Customers";
        var vm = new PagedListVm<CustomerListRowVm>
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
        return View(new CustomerUpsertVm { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerUpsertVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var cedula = vm.Cedula.Trim();
        var email = vm.Email.Trim().ToLowerInvariant();

        var cedExists = await _db.Customers.AnyAsync(x => x.Cedula == cedula, ct);
        if (cedExists)
        {
            ModelState.AddModelError(nameof(vm.Cedula), "La cédula ya está registrada.");
            return View(vm);
        }

        var emailExists = await _db.Customers.AnyAsync(x => x.Email == email, ct);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(vm.Email), "El email ya está registrado.");
            return View(vm);
        }

        var entity = new Customer
        {
            Cedula = cedula,
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(vm.Phone) ? null : vm.Phone.Trim(),
            Address = string.IsNullOrWhiteSpace(vm.Address) ? null : vm.Address.Trim(),
            IsActive = vm.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Customers.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Cliente creado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var c = await _db.Customers.SingleOrDefaultAsync(x => x.CustomerId == id, ct);
        if (c is null) return NotFound();

        return View(new CustomerUpsertVm
        {
            CustomerId = c.CustomerId,
            Cedula = c.Cedula,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            IsActive = c.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomerUpsertVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.Customers.SingleOrDefaultAsync(x => x.CustomerId == vm.CustomerId, ct);
        if (entity is null) return NotFound();

        var cedula = vm.Cedula.Trim();
        var email = vm.Email.Trim().ToLowerInvariant();

        var cedExists = await _db.Customers.AnyAsync(x => x.CustomerId != vm.CustomerId && x.Cedula == cedula, ct);
        if (cedExists)
        {
            ModelState.AddModelError(nameof(vm.Cedula), "La cédula ya está registrada.");
            return View(vm);
        }

        var emailExists = await _db.Customers.AnyAsync(x => x.CustomerId != vm.CustomerId && x.Email == email, ct);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(vm.Email), "El email ya está registrado.");
            return View(vm);
        }

        entity.Cedula = cedula;
        entity.FirstName = vm.FirstName.Trim();
        entity.LastName = vm.LastName.Trim();
        entity.Email = email;
        entity.Phone = string.IsNullOrWhiteSpace(vm.Phone) ? null : vm.Phone.Trim();
        entity.Address = string.IsNullOrWhiteSpace(vm.Address) ? null : vm.Address.Trim();
        entity.IsActive = vm.IsActive;

        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Cliente actualizado.";
        return RedirectToAction(nameof(Index), new { q = vm.Cedula });
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var c = await _db.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == id, ct);
        if (c is null) return NotFound();
        return View(c);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var c = await _db.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == id, ct);
        if (c is null) return NotFound();
        return View(c);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var c = await _db.Customers.SingleOrDefaultAsync(x => x.CustomerId == id, ct);
        if (c is null) return NotFound();

        var used = await _db.Sales.AnyAsync(x => x.CustomerId == id, ct);
        if (used)
        {
            TempData["ToastError"] = "No se puede eliminar: el cliente ya tiene ventas asociadas.";
            return RedirectToAction(nameof(Index));
        }

        _db.Customers.Remove(c);
        await _db.SaveChangesAsync(ct);

        TempData["Toast"] = "Cliente eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string q, CancellationToken ct)
    {
        var term = (q ?? "").Trim();
        if (term.Length < 2) return Json(Array.Empty<object>());

        var query = _db.Customers.AsNoTracking().Where(x => x.IsActive);

        if (term.All(char.IsDigit))
        {
            query = query.Where(x => x.Cedula.Contains(term));
        }
        else
        {
            query = query.Where(x =>
                x.FirstName.Contains(term) ||
                x.LastName.Contains(term) ||
                (x.FirstName + " " + x.LastName).Contains(term));
        }

        var list = await query
            .OrderBy(x => x.Cedula)
            .Take(10)
            .Select(x => new
            {
                id = x.CustomerId,
                cedula = x.Cedula,
                fullName = x.FirstName + " " + x.LastName,
                email = x.Email,
                phone = x.Phone
            })
            .ToListAsync(ct);

        return Json(list);
    }
}

public sealed class CustomerListRowVm
{
    public int CustomerId { get; set; }
    public string Cedula { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}
