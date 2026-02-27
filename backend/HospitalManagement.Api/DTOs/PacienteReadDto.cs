namespace HospitalManagement.Api.DTOs;

public class PacienteReadDto
{
    public int PacienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; } = string.Empty;
}
