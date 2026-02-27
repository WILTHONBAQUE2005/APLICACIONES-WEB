namespace HospitalManagement.Api.DTOs;

public class CitaMedicaReadDto
{
    public int CitaId { get; set; }
    public int PacienteId { get; set; }
    public int DoctorId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string PacienteNombreCompleto { get; set; } = string.Empty;
    public string DoctorNombreCompleto { get; set; } = string.Empty;
    public string EspecialidadDoctor { get; set; } = string.Empty;
}
