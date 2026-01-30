using System.ComponentModel.DataAnnotations;

namespace SalesPro.Web.ViewModels;

public sealed class ProductUpsertVm
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(32)]
    [RegularExpression("^[A-Za-z0-9-]{2,32}$")]
    public string Code { get; set; } = "";

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 99999999)]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;
}
