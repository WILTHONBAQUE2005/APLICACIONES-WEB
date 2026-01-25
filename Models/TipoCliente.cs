using System.ComponentModel.DataAnnotations;

namespace ClientesCrudWeb.Models;

public class TipoCliente
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Nombre { get; set; } = string.Empty;

    public List<Cliente> Clientes { get; set; } = new();
}
