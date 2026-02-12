using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaginaWeb.Core.Interfaces;
using PaginaWeb.Infrastructure.Data;
using PaginaWeb.Infrastructure.Repositories;
using PaginaWeb.Infrastructure.Security;
using PaginaWeb.Infrastructure.Seed;

namespace PaginaWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("MariaDb") ?? throw new InvalidOperationException("Missing connection string: MariaDb");
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseMySql(cs, ServerVersion.AutoDetect(cs), mysql =>
            {
                mysql.EnableRetryOnFailure(3);
            });
        });

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
