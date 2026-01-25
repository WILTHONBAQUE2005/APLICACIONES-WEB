using System.ComponentModel.DataAnnotations;

namespace ClientesCrudWeb.Models;

public class Cliente
{
    public int Id { get; set; }

    
    [MaxLength(10)]
    public string? Cedula { get; set; }

    
    [Required, MaxLength(80)]
    public string Nombres { get; set; } = string.Empty;

    
    [Required, MaxLength(80)]
    public string Apellidos { get; set; } = string.Empty;

    
    public DateTime? FechaNacimiento { get; set; }

    
    [MaxLength(20)]
    public string? Telefono { get; set; }

    
    [MaxLength(120)]
    public string? Email { get; set; }

    
    [MaxLength(200)]
    public string? Direccion { get; set; }

    
    [Required]
    public int CiudadId { get; set; }
    public Ciudad? Ciudad { get; set; }

    
    [Required]
    public int CantonId { get; set; }
    public Canton? Canton { get; set; }

    
    [Required]
    public int TipoClienteId { get; set; }
    public TipoCliente? TipoCliente { get; set; }

    
    public bool Activo { get; set; } = true;

    
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
