
using System.ComponentModel.DataAnnotations;

namespace SistemaPedidos.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(32)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Description { get; set; }

    [Range(0.0, 9999999.99)]
    public decimal UnitPrice { get; set; }

    [Range(0, 999999)]
    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
