
using System.ComponentModel.DataAnnotations;

namespace SistemaPedidos.Models;

public class Address
{
    public int AddressId { get; set; }

    [Required]
    [StringLength(160)]
    public string Line1 { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Province { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Country { get; set; } = "Ecuador";

    [StringLength(16)]
    public string? ZipCode { get; set; }

    public bool IsDefault { get; set; } = false;

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
