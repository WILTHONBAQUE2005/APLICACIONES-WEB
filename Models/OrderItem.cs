
using System.ComponentModel.DataAnnotations;

namespace SistemaPedidos.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }

    [Range(1, 999999)]
    public int Quantity { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal UnitPrice { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal LineTotal { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
