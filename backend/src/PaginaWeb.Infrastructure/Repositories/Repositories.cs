using Microsoft.EntityFrameworkCore;
using PaginaWeb.Core.Entities;
using PaginaWeb.Core.Interfaces;
using PaginaWeb.Infrastructure.Data;

namespace PaginaWeb.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<AppUser?> FindByEmailAsync(string email, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class ClientRepository : IClientRepository
{
    private readonly AppDbContext _db;
    public ClientRepository(AppDbContext db) => _db = db;

    public Task<List<Client>> ListAsync(CancellationToken ct) =>
        _db.Clients.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public Task<Client?> GetAsync(Guid id, CancellationToken ct) =>
        _db.Clients.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AddAsync(Client entity, CancellationToken ct) =>
        _db.Clients.AddAsync(entity, ct).AsTask();

    public Task DeleteAsync(Client entity, CancellationToken ct)
    {
        _db.Clients.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public ProductRepository(AppDbContext db) => _db = db;

    public Task<List<Product>> ListAsync(CancellationToken ct) =>
        _db.Products.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public Task<Product?> GetAsync(Guid id, CancellationToken ct) =>
        _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AddAsync(Product entity, CancellationToken ct) =>
        _db.Products.AddAsync(entity, ct).AsTask();

    public Task DeleteAsync(Product entity, CancellationToken ct)
    {
        _db.Products.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
