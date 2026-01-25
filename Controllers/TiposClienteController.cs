using ClientesCrudWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesCrudWeb.Controllers;

[ApiController]
[Route("api/tiposcliente")]
public class TiposClienteController : ControllerBase
{
    private readonly AppDbContext _db;
    public TiposClienteController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.TiposCliente
            .AsNoTracking()
            .OrderBy(t => t.Nombre)
            .Select(t => new { t.Id, t.Nombre })
            .ToListAsync();

        return Ok(data);
    }
}
