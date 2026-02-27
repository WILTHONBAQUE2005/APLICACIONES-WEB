namespace HospitalManagement.Api.Models;

public class Paciente
{
    public int PacienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; } = string.Empty;

    public ICollection<CitaMedica> CitasMedicas { get; set; } = new List<CitaMedica>();
}
