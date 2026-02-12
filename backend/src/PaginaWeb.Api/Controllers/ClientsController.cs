using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaWeb.Core.Dtos;
using PaginaWeb.Core.Entities;
using PaginaWeb.Core.Interfaces;

namespace PaginaWeb.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public sealed class ClientsController : ControllerBase
{
    private readonly IClientRepository _repo;

    public ClientsController(IClientRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<List<ClientListItem>>> List(CancellationToken ct)
    {
        var data = await _repo.ListAsync(ct);
        var items = data.Select(x => new ClientListItem(
            x.Id, x.DocumentType, x.DocumentNumber, x.FirstName, x.LastName, x.Email, x.Phone, x.AddressLine1, x.City, x.Notes, x.IsActive, x.CreatedAtUtc, x.UpdatedAtUtc
        )).ToList();

        return items;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Client>> Get(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(id, ct);
        return entity is null ? NotFound() : entity;
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] ClientUpsert req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DocumentNumber) || string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Missing required fields" });

        var entity = new Client
        {
            DocumentType = req.DocumentType?.Trim().ToUpperInvariant() ?? "CEDULA",
            DocumentNumber = req.DocumentNumber.Trim(),
            FirstName = req.FirstName.Trim(),
            LastName = req.LastName.Trim(),
            Email = (req.Email ?? string.Empty).Trim().ToLowerInvariant(),
            Phone = (req.Phone ?? string.Empty).Trim(),
            AddressLine1 = (req.AddressLine1 ?? string.Empty).Trim(),
            City = (req.City ?? string.Empty).Trim(),
            Notes = (req.Notes ?? string.Empty).Trim(),
            IsActive = req.IsActive
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveAsync(ct);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Client>> Update(Guid id, [FromBody] ClientUpsert req, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(id, ct);
        if (entity is null) return NotFound();

        entity.DocumentType = req.DocumentType?.Trim().ToUpperInvariant() ?? entity.DocumentType;
        entity.DocumentNumber = (req.DocumentNumber ?? entity.DocumentNumber).Trim();
        entity.FirstName = (req.FirstName ?? entity.FirstName).Trim();
        entity.LastName = (req.LastName ?? entity.LastName).Trim();
        entity.Email = (req.Email ?? entity.Email).Trim().ToLowerInvariant();
        entity.Phone = (req.Phone ?? entity.Phone).Trim();
        entity.AddressLine1 = (req.AddressLine1 ?? entity.AddressLine1).Trim();
        entity.City = (req.City ?? entity.City).Trim();
        entity.Notes = (req.Notes ?? entity.Notes).Trim();
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
