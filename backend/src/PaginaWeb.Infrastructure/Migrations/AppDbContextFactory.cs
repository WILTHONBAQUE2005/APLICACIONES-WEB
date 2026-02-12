using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaginaWeb.Infrastructure.Data;

namespace PaginaWeb.Infrastructure.Migrations;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("PAGINAWEB_MARIADB")
                 ?? "server=localhost;port=3306;database=pagina_web;user=root;password=;TreatTinyAsBoolean=true;SslMode=None;";
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(cs, ServerVersion.AutoDetect(cs))
            .Options;
        return new AppDbContext(opt);
    }
}
