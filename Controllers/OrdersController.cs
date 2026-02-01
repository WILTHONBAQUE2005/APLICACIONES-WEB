
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models.Enums;
using SistemaPedidos.Services;
using SistemaPedidos.ViewModels;

namespace SistemaPedidos.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _db;
    private readonly OrderService _orders;

    public OrdersController(AppDbContext db, OrderService orders)
    {
        _db = db;
        _orders = orders;
    }

    public async Task<IActionResult> Index(OrderIndexFilterVm filter)
    {
        var query = _db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(s) ||
                (o.Customer != null && (o.Customer.FirstName.Contains(s) || o.Customer.LastName.Contains(s) || o.Customer.DocumentNumber.Contains(s))));
        }

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);

        if (filter.From.HasValue)
            query = query.Where(o => o.OrderDate >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(o => o.OrderDate <= filter.To.Value);

        var list = await query.OrderByDescending(o => o.OrderDate).Take(400).ToListAsync();

        ViewBag.StatusList = new SelectList(Enum.GetValues<OrderStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", filter.Status);

        return View((filter, list));
    }

    public async Task<IActionResult> Create()
    {
        await PrepareCreateAsync();
        return View(new OrderCreateVm { OrderDate = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateVm model)
    {
        if (model.Items == null || model.Items.Count == 0)
            ModelState.AddModelError(string.Empty, "Agrega al menos un producto al pedido.");

        if (!ModelState.IsValid)
        {
            await PrepareCreateAsync(model.CustomerId);
            return View(model);
        }

        try
        {
            var orderId = await _orders.CreateAsync(model);
            TempData["ok"] = "Pedido creado.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PrepareCreateAsync(model.CustomerId);
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payments)
            .Include(o => o.Shipment)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        var vm = new OrderDetailsVm
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Customer = order.Customer != null ? order.Customer.FullName : "",
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Tax = order.Tax,
            Total = order.Total,
            Notes = order.Notes,
            Items = order.Items.Select(i => new OrderDetailsItemVm
            {
                Sku = i.Product != null ? i.Product.Sku : "",
                Product = i.Product != null ? i.Product.Name : "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList(),
            Payments = order.Payments.OrderByDescending(p => p.PaidAt).Select(p => new OrderPaymentVm
            {
                PaidAt = p.PaidAt,
                Method = p.Method.ToString(),
                Amount = p.Amount,
                Reference = p.Reference
            }).ToList(),
            Shipment = order.Shipment == null ? null : new ShipmentVm
            {
                Status = order.Shipment.Status.ToString(),
                Carrier = order.Shipment.Carrier,
                TrackingNumber = order.Shipment.TrackingNumber,
                ShipTo = order.Shipment.ShipTo,
                ShippedAt = order.Shipment.ShippedAt,
                DeliveredAt = order.Shipment.DeliveredAt
            },
            NewPayment = new PaymentCreateVm { OrderId = order.OrderId, Amount = Math.Max(0.01m, order.Total) },
            ShipmentUpdate = new ShipmentUpdateVm
            {
                OrderId = order.OrderId,
                Carrier = order.Shipment?.Carrier,
                TrackingNumber = order.Shipment?.TrackingNumber,
                ShipTo = order.Shipment?.ShipTo,
                Status = order.Shipment?.Status ?? ShipmentStatus.Pending
            }
        };

        ViewBag.OrderStatusList = new SelectList(Enum.GetValues<OrderStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", vm.Status);
        ViewBag.PaymentMethodList = new SelectList(Enum.GetValues<PaymentMethod>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", vm.NewPayment.Method);
        ViewBag.ShipmentStatusList = new SelectList(Enum.GetValues<ShipmentStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", vm.ShipmentUpdate.Status);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPayment(PaymentCreateVm input)
    {
        try
        {
            await _orders.AddPaymentAsync(input);
            TempData["ok"] = "Pago registrado.";
        }
        catch (Exception ex)
        {
            TempData["err"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = input.OrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShipment(ShipmentUpdateVm input)
    {
        try
        {
            await _orders.UpdateShipmentAsync(input);
            TempData["ok"] = "Envío actualizado.";
        }
        catch (Exception ex)
        {
            TempData["err"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = input.OrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus status)
    {
        try
        {
            await _orders.ChangeStatusAsync(orderId, status);
            TempData["ok"] = "Estado actualizado.";
        }
        catch (Exception ex)
        {
            TempData["err"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    private async Task PrepareCreateAsync(int? selectedCustomerId = null)
    {
        var customers = await _db.Customers.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToListAsync();
        var products = await _db.Products.AsNoTracking().Where(p => p.IsActive).Include(p => p.Category).OrderBy(p => p.Name).ToListAsync();

        ViewBag.Customers = new SelectList(customers.Select(c => new { c.CustomerId, Name = c.FullName + " (" + c.DocumentNumber + ")" }), "CustomerId", "Name", selectedCustomerId);
        ViewBag.Products = products.Select(p => new
        {
            p.ProductId,
            p.Sku,
            p.Name,
            Price = p.UnitPrice,
            Stock = p.Stock,
            Category = p.Category != null ? p.Category.Name : ""
        }).ToList();
    }
}
