
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models.Enums;
using SistemaPedidos.ViewModels;

namespace SistemaPedidos.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardVm> GetAsync()
    {
        var now = DateTime.UtcNow;
        var from = now.AddDays(-30);

        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Where(o => o.OrderDate >= from && o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Draft)
            .Select(o => new
            {
                o.OrderId,
                o.OrderNumber,
                Customer = o.Customer != null ? (o.Customer.FirstName + " " + o.Customer.LastName) : "",
                o.OrderDate,
                o.Status,
                o.Total
            })
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var vm = new DashboardVm
        {
            SalesLast30Days = orders.Sum(x => x.Total),
            OrdersOpen = await _db.Orders.AsNoTracking().CountAsync(o =>
                o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing || o.Status == OrderStatus.Shipped),
            CustomersActive = await _db.Customers.AsNoTracking().CountAsync(c => c.IsActive),
            LowStockProducts = await _db.Products.AsNoTracking().CountAsync(p => p.IsActive && p.Stock <= 5),
            RecentOrders = orders.Take(8).Select(x => new RecentOrderVm
            {
                OrderId = x.OrderId,
                OrderNumber = x.OrderNumber,
                Customer = x.Customer,
                Date = x.OrderDate,
                Status = x.Status.ToString(),
                Total = x.Total
            }).ToList()
        };

        var items = await _db.OrderItems
            .AsNoTracking()
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .Where(oi => oi.Order != null && oi.Order.OrderDate >= from && oi.Order.Status != OrderStatus.Cancelled && oi.Order.Status != OrderStatus.Draft)
            .Select(oi => new
            {
                ProductId = oi.ProductId,
                Sku = oi.Product != null ? oi.Product.Sku : "",
                Name = oi.Product != null ? oi.Product.Name : "",
                oi.Quantity,
                oi.LineTotal
            })
            .ToListAsync();

        vm.TopProducts = items
            .GroupBy(x => new { x.ProductId, x.Sku, x.Name })
            .Select(g => new TopProductVm
            {
                ProductId = g.Key.ProductId,
                Sku = g.Key.Sku,
                Name = g.Key.Name,
                Units = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.LineTotal)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(8)
            .ToList();

        vm.SalesSeries = orders
            .GroupBy(x => x.OrderDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SalesPointVm
            {
                Day = g.Key.ToString("MM-dd"),
                Total = g.Sum(x => x.Total)
            })
            .ToList();

        return vm;
    }
}
