using Microsoft.EntityFrameworkCore;
using NetCoreApi.Data;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var cs = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(cs));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    Console.WriteLine($"ENV: {app.Environment.EnvironmentName}");

    
    
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);


app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
