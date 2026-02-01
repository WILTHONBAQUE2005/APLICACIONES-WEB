
using System.ComponentModel.DataAnnotations;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Models;

public class Order
{
    public int OrderId { get; set; }

    [Required]
    [StringLength(20)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    [Range(0.0, 99999999.99)]
    public decimal Subtotal { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal Discount { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal Tax { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal Total { get; set; }

    [StringLength(400)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public Shipment? Shipment { get; set; }
}
