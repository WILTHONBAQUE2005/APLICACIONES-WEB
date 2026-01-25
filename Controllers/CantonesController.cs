using ClientesCrudWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesCrudWeb.Controllers;

[ApiController]
[Route("api/cantones")]
public class CantonesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CantonesController(AppDbContext db) => _db = db;

    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? ciudadId)
    {
        var query = _db.Cantones.AsNoTracking();

        if (ciudadId.HasValue)
            query = query.Where(c => c.CiudadId == ciudadId.Value);

        var data = await query
            .OrderBy(c => c.Nombre)
            .Select(c => new { c.Id, c.Nombre, c.CiudadId })
            .ToListAsync();

        return Ok(data);
    }
}
