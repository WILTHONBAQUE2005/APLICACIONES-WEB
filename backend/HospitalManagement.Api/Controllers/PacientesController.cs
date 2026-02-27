using HospitalManagement.Api.Data;
using HospitalManagement.Api.DTOs;
using HospitalManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Api.Controllers;

[ApiController]
[Route("api/pacientes")]
public class PacientesController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public PacientesController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PacienteReadDto>>> GetAll()
    {
        var pacientes = await _context.Pacientes
            .AsNoTracking()
            .OrderBy(x => x.Apellido)
            .ThenBy(x => x.Nombre)
            .Select(x => new PacienteReadDto
            {
                PacienteId = x.PacienteId,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                FechaNacimiento = x.FechaNacimiento,
                Telefono = x.Telefono
            })
            .ToListAsync();

        return Ok(pacientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PacienteReadDto>> GetById(int id)
    {
        var paciente = await _context.Pacientes
            .AsNoTracking()
            .Where(x => x.PacienteId == id)
            .Select(x => new PacienteReadDto
            {
                PacienteId = x.PacienteId,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                FechaNacimiento = x.FechaNacimiento,
                Telefono = x.Telefono
            })
            .FirstOrDefaultAsync();

        if (paciente is null)
        {
            return NotFound(new { mensaje = "Paciente no encontrado." });
        }

        return Ok(paciente);
    }

    [HttpPost]
    public async Task<ActionResult<PacienteReadDto>> Create(PacienteCreateUpdateDto dto)
    {
        var entity = new Paciente
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            FechaNacimiento = dto.FechaNacimiento.Date,
            Telefono = dto.Telefono.Trim()
        };

        _context.Pacientes.Add(entity);
        await _context.SaveChangesAsync();

        var result = new PacienteReadDto
        {
            PacienteId = entity.PacienteId,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            FechaNacimiento = entity.FechaNacimiento,
            Telefono = entity.Telefono
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.PacienteId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PacienteCreateUpdateDto dto)
    {
        var entity = await _context.Pacientes.FirstOrDefaultAsync(x => x.PacienteId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Paciente no encontrado." });
        }

        entity.Nombre = dto.Nombre.Trim();
        entity.Apellido = dto.Apellido.Trim();
        entity.FechaNacimiento = dto.FechaNacimiento.Date;
        entity.Telefono = dto.Telefono.Trim();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Pacientes.FirstOrDefaultAsync(x => x.PacienteId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Paciente no encontrado." });
        }

        var tieneCitas = await _context.CitasMedicas.AnyAsync(x => x.PacienteId == id);
        if (tieneCitas)
        {
            return Conflict(new { mensaje = "No se puede eliminar el paciente porque tiene citas asociadas." });
        }

        _context.Pacientes.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
