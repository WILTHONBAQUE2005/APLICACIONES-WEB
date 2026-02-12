namespace PaginaWeb.Core.Dtos;

public sealed record ProductListItem(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    string Description,
    string Barcode,
    string ImageUrl,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public sealed record ProductUpsert(
    string Sku,
    string Name,
    string? Category,
    string? Description,
    string? Barcode,
    string? ImageUrl,
    decimal Price,
    int Stock,
    bool IsActive
);
