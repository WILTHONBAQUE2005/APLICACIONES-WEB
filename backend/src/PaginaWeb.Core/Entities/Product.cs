namespace PaginaWeb.Core.Entities;

public sealed class Product : BaseEntity
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }
    public int Stock { get; set; }
}
