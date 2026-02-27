using HospitalManagement.Api.Data;
using HospitalManagement.Api.DTOs;
using HospitalManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Api.Controllers;

[ApiController]
[Route("api/citasmedicas")]
public class CitasMedicasController : ControllerBase
{
    private static readonly HashSet<string> EstadosPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "Programada",
        "Completada",
        "Cancelada"
    };

    private readonly HospitalDbContext _context;

    public CitasMedicasController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CitaMedicaReadDto>>> GetAll()
    {
        var citas = await _context.CitasMedicas
            .AsNoTracking()
            .OrderBy(x => x.FechaHora)
            .Select(x => new CitaMedicaReadDto
            {
                CitaId = x.CitaId,
                PacienteId = x.PacienteId,
                DoctorId = x.DoctorId,
                FechaHora = x.FechaHora,
                Motivo = x.Motivo,
                Estado = x.Estado,
                PacienteNombreCompleto = x.Paciente!.Nombre + " " + x.Paciente.Apellido,
                DoctorNombreCompleto = x.Doctor!.Nombre + " " + x.Doctor.Apellido,
                EspecialidadDoctor = x.Doctor.Especialidad
            })
            .ToListAsync();

        return Ok(citas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CitaMedicaReadDto>> GetById(int id)
    {
        var cita = await _context.CitasMedicas
            .AsNoTracking()
            .Where(x => x.CitaId == id)
            .Select(x => new CitaMedicaReadDto
            {
                CitaId = x.CitaId,
                PacienteId = x.PacienteId,
                DoctorId = x.DoctorId,
                FechaHora = x.FechaHora,
                Motivo = x.Motivo,
                Estado = x.Estado,
                PacienteNombreCompleto = x.Paciente!.Nombre + " " + x.Paciente.Apellido,
                DoctorNombreCompleto = x.Doctor!.Nombre + " " + x.Doctor.Apellido,
                EspecialidadDoctor = x.Doctor.Especialidad
            })
            .FirstOrDefaultAsync();

        if (cita is null)
        {
            return NotFound(new { mensaje = "Cita medica no encontrada." });
        }

        return Ok(cita);
    }

    [HttpPost]
    public async Task<ActionResult<CitaMedicaReadDto>> Create(CitaMedicaCreateUpdateDto dto)
    {
        var validationError = await ValidarCitaAsync(dto, null);
        if (validationError is not null)
        {
            return validationError;
        }

        var entity = new CitaMedica
        {
            PacienteId = dto.PacienteId,
            DoctorId = dto.DoctorId,
            FechaHora = dto.FechaHora,
            Motivo = dto.Motivo.Trim(),
            Estado = NormalizarEstado(dto.Estado)
        };

        _context.CitasMedicas.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _context.CitasMedicas
            .AsNoTracking()
            .Where(x => x.CitaId == entity.CitaId)
            .Select(x => new CitaMedicaReadDto
            {
                CitaId = x.CitaId,
                PacienteId = x.PacienteId,
                DoctorId = x.DoctorId,
                FechaHora = x.FechaHora,
                Motivo = x.Motivo,
                Estado = x.Estado,
                PacienteNombreCompleto = x.Paciente!.Nombre + " " + x.Paciente.Apellido,
                DoctorNombreCompleto = x.Doctor!.Nombre + " " + x.Doctor.Apellido,
                EspecialidadDoctor = x.Doctor.Especialidad
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.CitaId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CitaMedicaCreateUpdateDto dto)
    {
        var entity = await _context.CitasMedicas.FirstOrDefaultAsync(x => x.CitaId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Cita medica no encontrada." });
        }

        var validationError = await ValidarCitaAsync(dto, id);
        if (validationError is not null)
        {
            return validationError;
        }

        entity.PacienteId = dto.PacienteId;
        entity.DoctorId = dto.DoctorId;
        entity.FechaHora = dto.FechaHora;
        entity.Motivo = dto.Motivo.Trim();
        entity.Estado = NormalizarEstado(dto.Estado);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.CitasMedicas.FirstOrDefaultAsync(x => x.CitaId == id);
        if (entity is null)
        {
            return NotFound(new { mensaje = "Cita medica no encontrada." });
        }

        _context.CitasMedicas.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ObjectResult?> ValidarCitaAsync(CitaMedicaCreateUpdateDto dto, int? citaIdActual)
    {
        if (!EstadosPermitidos.Contains(dto.Estado))
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new { mensaje = "El estado de la cita no es valido." });
        }

        var pacienteExiste = await _context.Pacientes.AnyAsync(x => x.PacienteId == dto.PacienteId);
        if (!pacienteExiste)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new { mensaje = "El paciente seleccionado no existe." });
        }

        var doctorExiste = await _context.Doctores.AnyAsync(x => x.DoctorId == dto.DoctorId);
        if (!doctorExiste)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new { mensaje = "El doctor seleccionado no existe." });
        }

        var doctorOcupado = await _context.CitasMedicas.AnyAsync(x =>
            x.DoctorId == dto.DoctorId &&
            x.FechaHora == dto.FechaHora &&
            (!citaIdActual.HasValue || x.CitaId != citaIdActual.Value));

        if (doctorOcupado)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new { mensaje = "El doctor ya tiene una cita registrada en esa fecha y hora." });
        }

        var pacienteOcupado = await _context.CitasMedicas.AnyAsync(x =>
            x.PacienteId == dto.PacienteId &&
            x.FechaHora == dto.FechaHora &&
            (!citaIdActual.HasValue || x.CitaId != citaIdActual.Value));

        if (pacienteOcupado)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new { mensaje = "El paciente ya tiene una cita registrada en esa fecha y hora." });
        }

        return null;
    }

    private static string NormalizarEstado(string estado)
    {
        return estado.Trim().ToLowerInvariant() switch
        {
            "programada" => "Programada",
            "completada" => "Completada",
            "cancelada" => "Cancelada",
            _ => estado.Trim()
        };
    }
}
