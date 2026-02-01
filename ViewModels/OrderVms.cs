
using System.ComponentModel.DataAnnotations;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.ViewModels;

public class OrderCreateVm
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Range(0.0, 99999999.99)]
    public decimal Discount { get; set; }

    [StringLength(400)]
    public string? Notes { get; set; }

    [MinLength(1)]
    public List<OrderItemCreateVm> Items { get; set; } = new();
}

public class OrderItemCreateVm
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 999999)]
    public int Quantity { get; set; }

    [Range(0.0, 99999999.99)]
    public decimal UnitPrice { get; set; }
}

public class OrderIndexFilterVm
{
    public string? Search { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class PaymentCreateVm
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public PaymentMethod Method { get; set; }

    [Range(0.01, 99999999.99)]
    public decimal Amount { get; set; }

    public string? Reference { get; set; }
}

public class ShipmentUpdateVm
{
    [Required]
    public int OrderId { get; set; }

    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShipTo { get; set; }

    public ShipmentStatus Status { get; set; }
}

public class OrderDetailsVm
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Customer { get; set; } = string.Empty;

    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public string? Notes { get; set; }

    public List<OrderDetailsItemVm> Items { get; set; } = new();
    public List<OrderPaymentVm> Payments { get; set; } = new();
    public ShipmentVm? Shipment { get; set; }

    public PaymentCreateVm NewPayment { get; set; } = new();
    public ShipmentUpdateVm ShipmentUpdate { get; set; } = new();
}

public class OrderDetailsItemVm
{
    public string Sku { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class OrderPaymentVm
{
    public DateTime PaidAt { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

public class ShipmentVm
{
    public string Status { get; set; } = string.Empty;
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShipTo { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
