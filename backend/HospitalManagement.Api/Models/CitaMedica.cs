namespace HospitalManagement.Api.Models;

public class CitaMedica
{
    public int CitaId { get; set; }
    public int PacienteId { get; set; }
    public int DoctorId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Programada";

    public Paciente? Paciente { get; set; }
    public Doctor? Doctor { get; set; }
}
