using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Api.DTOs;

public class DoctorCreateUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Especialidad { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;
}
