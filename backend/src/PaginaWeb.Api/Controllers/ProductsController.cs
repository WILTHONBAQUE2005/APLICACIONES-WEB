using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaWeb.Core.Dtos;
using PaginaWeb.Core.Entities;
using PaginaWeb.Core.Interfaces;

namespace PaginaWeb.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;

    public ProductsController(IProductRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<List<ProductListItem>>> List(CancellationToken ct)
    {
        var data = await _repo.ListAsync(ct);
        var items = data.Select(x => new ProductListItem(
    x.Id,
    x.Sku,
    x.Name,
    x.Category ?? string.Empty,
    x.Description ?? string.Empty,
    x.Barcode ?? string.Empty,
    x.ImageUrl ?? string.Empty,
    x.Price,
    x.Stock,
    x.IsActive,
    x.CreatedAtUtc,
    x.UpdatedAtUtc
)).ToList();


        return items;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> Get(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(id, ct);
        return entity is null ? NotFound() : entity;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] ProductUpsert req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Sku) || string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Missing required fields" });

        if (req.Price < 0) return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid price" });
        if (req.Stock < 0) return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid stock" });

        var entity = new Product
        {
            Sku = req.Sku.Trim(),
            Name = req.Name.Trim(),
            Category = (req.Category ?? string.Empty).Trim(),
            Description = (req.Description ?? string.Empty).Trim(),
            Barcode = (req.Barcode ?? string.Empty).Trim(),
            ImageUrl = (req.ImageUrl ?? string.Empty).Trim(),
            Price = req.Price,
            Stock = req.Stock,
            IsActive = req.IsActive
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveAsync(ct);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Product>> Update(Guid id, [FromBody] ProductUpsert req, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(id, ct);
        if (entity is null) return NotFound();

        entity.Sku = (req.Sku ?? entity.Sku).Trim();
        entity.Name = (req.Name ?? entity.Name).Trim();
        entity.Category = (req.Category ?? entity.Category ?? string.Empty).Trim();
entity.Description = (req.Description ?? entity.Description ?? string.Empty).Trim();
entity.Barcode = (req.Barcode ?? entity.Barcode ?? string.Empty).Trim();
entity.ImageUrl = (req.ImageUrl ?? entity.ImageUrl ?? string.Empty).Trim();

        entity.Price = req.Price;
        entity.Stock = req.Stock;
        entity.IsActive = req.IsActive;

        await _repo.SaveAsync(ct);
        return entity;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(id, ct);
        if (entity is null) return NotFound();

        entity.IsActive = false;
        await _repo.SaveAsync(ct);
        return NoContent();
    }
	[HttpPost("{id:guid}/activate")]
public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
{
    var entity = await _repo.GetAsync(id, ct);
    if (entity is null) return NotFound();

    entity.IsActive = true;
    await _repo.SaveAsync(ct);

    return NoContent();
}


}
