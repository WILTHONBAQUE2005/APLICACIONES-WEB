using Microsoft.EntityFrameworkCore;
using SalesPro.Application.Services;
using SalesPro.Infrastructure.Persistence;
using SalesPro.Infrastructure.Services;
using SalesPro.Web;
using SalesPro.Web.Binding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

builder.Services.Configure<SalesOptions>(builder.Configuration.GetSection("SalesOptions"));

builder.Services.AddDbContext<SalesDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("MariaDb");
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

builder.Services.AddScoped<ISalesService, SalesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
