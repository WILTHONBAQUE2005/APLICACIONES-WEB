using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Api.DTOs;

public class CitaMedicaCreateUpdateDto
{
    [Required]
    public int PacienteId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime FechaHora { get; set; }

    [Required]
    [StringLength(250)]
    public string Motivo { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Programada";
}
