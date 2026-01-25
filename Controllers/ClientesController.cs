using System.Text.RegularExpressions;
using ClientesCrudWeb.Data;
using ClientesCrudWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesCrudWeb.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientesController(AppDbContext db)
    {
        _db = db;
    }

    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteListDto>>> GetAll([FromQuery] bool includeInactivos = false)
    {
        var query = _db.Clientes
            .AsNoTracking()
            .Include(c => c.Ciudad)
            .Include(c => c.Canton)
            .Include(c => c.TipoCliente)
            .AsQueryable();

        if (!includeInactivos)
            query = query.Where(c => c.Activo);

        var data = await query
            .OrderByDescending(c => c.Id)
            .Select(c => new ClienteListDto
            {
                Id = c.Id,
                Cedula = c.Cedula,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                FechaNacimiento = c.FechaNacimiento,
                Telefono = c.Telefono,
                Email = c.Email,
                Direccion = c.Direccion,
                CiudadId = c.CiudadId,
                Ciudad = c.Ciudad != null ? c.Ciudad.Provincia : "",
                CantonId = c.CantonId,
                Canton = c.Canton != null ? c.Canton.Nombre : "",
                TipoClienteId = c.TipoClienteId,
                TipoCliente = c.TipoCliente != null ? c.TipoCliente.Nombre : "",
                Activo = c.Activo,
                FechaRegistro = c.FechaRegistro
            })
            .ToListAsync();

        return Ok(data);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteUpsertDto>> GetById(int id)
    {
        var c = await _db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return NotFound();

        return Ok(new ClienteUpsertDto
        {
            Cedula = c.Cedula,
            Nombres = c.Nombres,
            Apellidos = c.Apellidos,
            FechaNacimiento = c.FechaNacimiento,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            CiudadId = c.CiudadId,
            CantonId = c.CantonId,
            TipoClienteId = c.TipoClienteId,
            Activo = c.Activo
        });
    }

    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ClienteUpsertDto dto)
    {
        var validation = await Validate(dto, idToExclude: null);
        if (validation != null) return validation;

        var entity = new Cliente
        {
            Cedula = NormalizeNullable(dto.Cedula),
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = NormalizeNullable(dto.Telefono),
            Email = NormalizeNullable(dto.Email),
            Direccion = NormalizeNullable(dto.Direccion),
            CiudadId = dto.CiudadId,
            CantonId = dto.CantonId,
            TipoClienteId = dto.TipoClienteId,
            Activo = dto.Activo,
            FechaRegistro = DateTime.Now
        };

        _db.Clientes.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.Id);
    }

    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] ClienteUpsertDto dto)
    {
        var validation = await Validate(dto, idToExclude: id);
        if (validation != null) return validation;

        var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.Cedula = NormalizeNullable(dto.Cedula);
        entity.Nombres = dto.Nombres.Trim();
        entity.Apellidos = dto.Apellidos.Trim();
        entity.FechaNacimiento = dto.FechaNacimiento;
        entity.Telefono = NormalizeNullable(dto.Telefono);
        entity.Email = NormalizeNullable(dto.Email);
        entity.Direccion = NormalizeNullable(dto.Direccion);
        entity.CiudadId = dto.CiudadId;
        entity.CantonId = dto.CantonId;
        entity.TipoClienteId = dto.TipoClienteId;
        entity.Activo = dto.Activo;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.Activo = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpPut("{id:int}/activar")]
    public async Task<ActionResult> Activar(int id)
    {
        var entity = await _db.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.Activo = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    
    private static readonly Regex PhoneRegex = new(
        @"^0\d{8,9}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private async Task<ActionResult?> Validate(ClienteUpsertDto dto, int? idToExclude)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Nombres))
            return BadRequest(new { message = "El campo 'Nombres' es obligatorio." });

        if (string.IsNullOrWhiteSpace(dto.Apellidos))
            return BadRequest(new { message = "El campo 'Apellidos' es obligatorio." });

        if (dto.CiudadId <= 0)
            return BadRequest(new { message = "Debe seleccionar una ciudad." });

        if (dto.CantonId <= 0)
            return BadRequest(new { message = "Debe seleccionar un cantón." });

        if (dto.TipoClienteId <= 0)
            return BadRequest(new { message = "Debe seleccionar un tipo de cliente." });

        
        if (!string.IsNullOrWhiteSpace(dto.Cedula))
        {
            var ced = dto.Cedula.Trim();
            if (!Regex.IsMatch(ced, @"^\d{10}$") || !IsCedulaEcuadorValida(ced))
                return BadRequest(new { message = "La cédula no es válida (Ecuador: 10 dígitos + verificación)." });

            var cedulaExists = await _db.Clientes
                .AsNoTracking()
                .AnyAsync(c => c.Cedula == ced && (!idToExclude.HasValue || c.Id != idToExclude.Value));

            if (cedulaExists)
                return Conflict(new { message = "Ya existe un cliente con esa cédula." });
        }

        
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            if (!EmailRegex.IsMatch(email))
                return BadRequest(new { message = "El email no tiene un formato válido." });

            var emailExists = await _db.Clientes
                .AsNoTracking()
                .AnyAsync(c => c.Email != null && c.Email.ToLower() == email && (!idToExclude.HasValue || c.Id != idToExclude.Value));

            if (emailExists)
                return Conflict(new { message = "Ya existe un cliente con ese email." });
        }

        
        if (!string.IsNullOrWhiteSpace(dto.Telefono))
        {
            var phone = dto.Telefono.Trim().Replace(" ", "").Replace("-", "");
            if (!PhoneRegex.IsMatch(phone))
                return BadRequest(new { message = "El teléfono no es válido. Usa formato 0XXXXXXXXX (9-10 dígitos)." });

            dto.Telefono = phone;
        }

        
        var cantonOk = await _db.Cantones
            .AsNoTracking()
            .AnyAsync(c => c.Id == dto.CantonId && c.CiudadId == dto.CiudadId);

        if (!cantonOk)
            return BadRequest(new { message = "El cantón seleccionado no pertenece a la ciudad elegida." });

        return null;
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    
    
    private static bool IsCedulaEcuadorValida(string cedula)
    {
        if (cedula.Length != 10) return false;

        var provincia = int.Parse(cedula.Substring(0, 2));
        if (provincia < 1 || provincia > 24) return false;

        var tercerDigito = int.Parse(cedula.Substring(2, 1));
        if (tercerDigito < 0 || tercerDigito > 5) return false;

        var total = 0;
        var coef = new[] { 2, 1, 2, 1, 2, 1, 2, 1, 2 };

        for (var i = 0; i < 9; i++)
        {
            var digit = cedula[i] - '0';
            var prod = digit * coef[i];
            if (prod >= 10) prod -= 9;
            total += prod;
        }

        var mod = total % 10;
        var check = mod == 0 ? 0 : 10 - mod;
        var last = cedula[9] - '0';

        return check == last;
    }

    public class ClienteUpsertDto
    {
        public string? Cedula { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public int CiudadId { get; set; }
        public int CantonId { get; set; }
        public int TipoClienteId { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class ClienteListDto
    {
        public int Id { get; set; }
        public string? Cedula { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public int CiudadId { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public int CantonId { get; set; }
        public string Canton { get; set; } = string.Empty;
        public int TipoClienteId { get; set; }
        public string TipoCliente { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
