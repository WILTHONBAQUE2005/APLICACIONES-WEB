
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Models;

namespace SistemaPedidos.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync())
            return;

        var categorias = new List<Category>
        {
            new() { Name = "Tecnologia", Description = "Dispositivos y accesorios", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Hogar", Description = "Articulos para casa", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Papeleria", Description = "Utiles escolares y oficina", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Alimentos", Description = "Consumo diario", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Salud", Description = "Higiene y cuidado", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        db.Categories.AddRange(categorias);

        var clientes = new List<Customer>
        {
            new() { DocumentNumber = "1723456789", FirstName = "Juan", LastName = "Vargas", Email = "juan.vargas@mail.com", Phone = "0991111111", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { DocumentNumber = "0912345678", FirstName = "Maria", LastName = "Paredes", Email = "maria.paredes@mail.com", Phone = "0982222222", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { DocumentNumber = "1109876543", FirstName = "Carlos", LastName = "Mendoza", Email = "carlos.mendoza@mail.com", Phone = "0973333333", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        db.Customers.AddRange(clientes);

        await db.SaveChangesAsync();

        var catTec = await db.Categories.FirstAsync(x => x.Name == "Tecnologia");
        var catHog = await db.Categories.FirstAsync(x => x.Name == "Hogar");
        var catPap = await db.Categories.FirstAsync(x => x.Name == "Papeleria");
        var catAli = await db.Categories.FirstAsync(x => x.Name == "Alimentos");
        var catSal = await db.Categories.FirstAsync(x => x.Name == "Salud");

        var productos = new List<Product>
        {
            new() { Sku = "TEC-0001", Name = "Mouse inalambrico", Description = "2.4G, ergonomico", UnitPrice = 15.99m, Stock = 40, CategoryId = catTec.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "TEC-0002", Name = "Teclado mecanico", Description = "Switch azul", UnitPrice = 49.90m, Stock = 20, CategoryId = catTec.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "TEC-0003", Name = "Audifonos bluetooth", Description = "Bateria 20h", UnitPrice = 29.50m, Stock = 35, CategoryId = catTec.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "HOG-0001", Name = "Foco LED 12W", Description = "Luz blanca", UnitPrice = 3.25m, Stock = 100, CategoryId = catHog.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "HOG-0002", Name = "Set sabanas 2 plazas", Description = "Microfibra", UnitPrice = 22.80m, Stock = 15, CategoryId = catHog.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "PAP-0001", Name = "Cuaderno universitario 100h", Description = "Cuadriculado", UnitPrice = 2.10m, Stock = 120, CategoryId = catPap.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "PAP-0002", Name = "Boligrafos pack 3", Description = "Tinta azul", UnitPrice = 1.50m, Stock = 200, CategoryId = catPap.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "ALI-0001", Name = "Cafe molido 250g", Description = "Tueste medio", UnitPrice = 4.75m, Stock = 60, CategoryId = catAli.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "ALI-0002", Name = "Galletas integrales", Description = "Paquete 200g", UnitPrice = 1.95m, Stock = 80, CategoryId = catAli.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "SAL-0001", Name = "Alcohol en gel 500ml", Description = "70% alcohol", UnitPrice = 2.85m, Stock = 90, CategoryId = catSal.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Sku = "SAL-0002", Name = "Vitamina C 1000mg 30u", Description = "Tabletas", UnitPrice = 6.40m, Stock = 50, CategoryId = catSal.CategoryId, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        db.Products.AddRange(productos);
        await db.SaveChangesAsync();

        var cliente = await db.Customers.FirstAsync();
        var prod1 = await db.Products.FirstAsync(x => x.Sku == "TEC-0001");
        var prod2 = await db.Products.FirstAsync(x => x.Sku == "PAP-0001");

        var order = new Order
        {
            OrderNumber = "P-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-0001",
            CustomerId = cliente.CustomerId,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            Subtotal = 15.99m + (2.10m * 3),
            Discount = 0m,
            Tax = 0m,
            Total = 15.99m + (2.10m * 3),
            Notes = "Pedido de prueba",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        order.Items.Add(new Models.OrderItem { ProductId = prod1.ProductId, Quantity = 1, UnitPrice = prod1.UnitPrice, LineTotal = prod1.UnitPrice });
        order.Items.Add(new Models.OrderItem { ProductId = prod2.ProductId, Quantity = 3, UnitPrice = prod2.UnitPrice, LineTotal = prod2.UnitPrice * 3 });

        db.Orders.Add(order);
        await db.SaveChangesAsync();
    }
}
