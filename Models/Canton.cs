using System.ComponentModel.DataAnnotations;

namespace ClientesCrudWeb.Models;

public class Canton
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public int CiudadId { get; set; }
    public Ciudad? Ciudad { get; set; }

    public List<Cliente> Clientes { get; set; } = new();
}
