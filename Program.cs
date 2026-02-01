
using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Data;
using SistemaPedidos.Services;

using System.Globalization;

var culture = new CultureInfo("es-EC");
culture.NumberFormat.NumberDecimalSeparator = ".";
culture.NumberFormat.NumberGroupSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewOptions(options => { });

var connectionString = builder.Configuration.GetConnectionString("MariaDb");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<InventoryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(db);
}

app.Run();
