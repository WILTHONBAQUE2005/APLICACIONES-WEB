namespace HospitalManagement.Api.DTOs;

public class DoctorReadDto
{
    public int DoctorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
