namespace PaginaWeb.Core.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IUserRepository
{
    Task<PaginaWeb.Core.Entities.AppUser?> FindByEmailAsync(string email, CancellationToken ct);
    Task<PaginaWeb.Core.Entities.AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(PaginaWeb.Core.Entities.AppUser user, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}

public interface IClientRepository
{
    Task<List<PaginaWeb.Core.Entities.Client>> ListAsync(CancellationToken ct);
    Task<PaginaWeb.Core.Entities.Client?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(PaginaWeb.Core.Entities.Client entity, CancellationToken ct);
    Task DeleteAsync(PaginaWeb.Core.Entities.Client entity, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}

public interface IProductRepository
{
    Task<List<PaginaWeb.Core.Entities.Product>> ListAsync(CancellationToken ct);
    Task<PaginaWeb.Core.Entities.Product?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(PaginaWeb.Core.Entities.Product entity, CancellationToken ct);
    Task DeleteAsync(PaginaWeb.Core.Entities.Product entity, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
