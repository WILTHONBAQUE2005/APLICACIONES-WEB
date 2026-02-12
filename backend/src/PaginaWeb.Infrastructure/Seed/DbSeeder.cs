using PaginaWeb.Core.Entities;
using PaginaWeb.Core.Interfaces;
using PaginaWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaginaWeb.Infrastructure.Seed;

public sealed class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public DbSeeder(AppDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (!await _db.Users.AnyAsync(ct))
        {
            var admin = new AppUser
            {
                Email = "admin@demo.com",
                FullName = "Administrador",
                Role = "admin",
                PasswordHash = _hasher.Hash("Admin123!")
            };
            await _db.Users.AddAsync(admin, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
