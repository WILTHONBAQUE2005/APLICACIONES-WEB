using HospitalManagement.Api.Data;
using HospitalManagement.Api.DTOs;
using HospitalManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Api.Controllers;

[ApiController]
[Route("api/doctores")]
public class DoctoresController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public DoctoresController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorReadDto>>> GetAll()
    {
        var doctores = await _context.Doctores
            .AsNoTracking()
            .OrderBy(x => x.Apellido)
            .ThenBy(x => x.Nombre)
            .Select(x => new DoctorReadDto
            {
                DoctorId = x.DoctorId,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Especialidad = x.Especialidad,
                Telefono = x.Telefono
            })
            .ToListAsync();

        return Ok(doctores);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorReadDto>> GetById(int id)
    {
        var doctor = await _context.Doctores
            .AsNoTracking()
            .Where(x => x.DoctorId == id)
            .Select(x => new DoctorReadDto
            {
                DoctorId = x.DoctorId,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Especialidad = x.Especialidad,
                Telefono = x.Telefono
            })
            .FirstOrDefaultAsync();

        if (doctor is null)
        {
            return NotFound(new { mensaje = "Doctor no encontrado." });
        }

        return Ok(doctor);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorReadDto>> Create(DoctorCreateUpdateDto dto)
    {
        var entity = new Doctor
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Especialidad = dto.Especialidad.Trim(),
            Telefono = dto.Telefono.Trim()
        };

        _context.Doctores.Add(entity);
        await _context.SaveChangesAsync();

        var result = new DoctorReadDto
        {
            DoctorId = entity.DoctorId,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            Especialidad = entity.Especialidad,
            Telefono = entity.Telefono
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.DoctorId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DoctorCreateUpdateDto dto)
    {
        var entity = await _context.Doctores.FirstOrDefaultAsync(x => x.DoctorId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Doctor no encontrado." });
        }

        entity.Nombre = dto.Nombre.Trim();
        entity.Apellido = dto.Apellido.Trim();
        entity.Especialidad = dto.Especialidad.Trim();
        entity.Telefono = dto.Telefono.Trim();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Doctores.FirstOrDefaultAsync(x => x.DoctorId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Doctor no encontrado." });
        }

        var tieneCitas = await _context.CitasMedicas.AnyAsync(x => x.DoctorId == id);
        if (tieneCitas)
        {
            return Conflict(new { mensaje = "No se puede eliminar el doctor porque tiene citas asociadas." });
        }

        _context.Doctores.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
