
using System.ComponentModel.DataAnnotations;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Models;

public class Shipment
{
    public int ShipmentId { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

    [StringLength(80)]
    public string? Carrier { get; set; }

    [StringLength(80)]
    public string? TrackingNumber { get; set; }

    [StringLength(200)]
    public string? ShipTo { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
}
