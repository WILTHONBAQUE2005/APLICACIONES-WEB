
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Models;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Services;

public class InventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AdjustAsync(int productId, int quantity, string? reason)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado.");

        product.Stock = Math.Max(0, product.Stock + quantity);

        _db.InventoryMovements.Add(new InventoryMovement
        {
            ProductId = product.ProductId,
            Type = InventoryMovementType.Adjust,
            Quantity = Math.Abs(quantity),
            Reason = string.IsNullOrWhiteSpace(reason) ? "Ajuste" : reason.Trim(),
            Reference = "AJUSTE",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
