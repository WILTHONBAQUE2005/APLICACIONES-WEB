using ClientesCrudWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesCrudWeb.Controllers;

[ApiController]
[Route("api/ciudades")]
public class CiudadesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CiudadesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Ciudades
            .AsNoTracking()
            .OrderBy(c => c.Provincia).ThenBy(c => c.Nombre)
            .Select(c => new { c.Id, c.Nombre, c.Provincia })
            .ToListAsync();

        return Ok(data);
    }
}
