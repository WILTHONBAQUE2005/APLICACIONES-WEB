
using System.ComponentModel.DataAnnotations;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Models;

public class InventoryMovement
{
    public int InventoryMovementId { get; set; }

    public InventoryMovementType Type { get; set; } = InventoryMovementType.Out;

    [Range(1, 999999)]
    public int Quantity { get; set; }

    [StringLength(120)]
    public string? Reason { get; set; }

    [StringLength(40)]
    public string? Reference { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
