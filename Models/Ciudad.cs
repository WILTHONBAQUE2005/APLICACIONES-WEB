using System.ComponentModel.DataAnnotations;

namespace ClientesCrudWeb.Models;

public class Ciudad
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Provincia { get; set; } = string.Empty;


    public List<Canton> Cantones { get; set; } = new();
    public List<Cliente> Clientes { get; set; } = new();
}
