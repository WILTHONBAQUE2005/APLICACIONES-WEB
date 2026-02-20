using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreApi.Data;
using NetCoreApi.Models;

namespace NetCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlumnosController : ControllerBase
{
    private readonly AppDbContext _context;
    

    public AlumnosController(AppDbContext context)
    {
      _context = context;

    } 

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlumnoModel>>> Get()
        => await _context.Set<AlumnoModel>().AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlumnoModel>> Get(int id)
    {
        var item = await _context.Set<AlumnoModel>().FindAsync(id);
        return item is null ? NotFound() : item;
    }

    [HttpPost]
    public async Task<ActionResult<AlumnoModel>> Post(AlumnoModel model)
    {
        _context.Set<AlumnoModel>().Add(model);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = model.id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, AlumnoModel model)
    {
        if (id != model.id) return BadRequest("Id no coincide.");

        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Set<AlumnoModel>().FindAsync(id);
        if (item is null) return NotFound();

        _context.Set<AlumnoModel>().Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
