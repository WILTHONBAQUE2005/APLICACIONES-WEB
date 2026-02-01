
namespace SistemaPedidos.ViewModels;

public class DashboardVm
{
    public decimal SalesLast30Days { get; set; }
    public int OrdersOpen { get; set; }
    public int CustomersActive { get; set; }
    public int LowStockProducts { get; set; }

    public List<RecentOrderVm> RecentOrders { get; set; } = new();
    public List<TopProductVm> TopProducts { get; set; } = new();
    public List<SalesPointVm> SalesSeries { get; set; } = new();
}

public class RecentOrderVm
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class TopProductVm
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Units { get; set; }
    public decimal Revenue { get; set; }
}

public class SalesPointVm
{
    public string Day { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
