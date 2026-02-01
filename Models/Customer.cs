
using System.ComponentModel.DataAnnotations;

namespace SistemaPedidos.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [Required]
    [StringLength(20)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public string FullName => $"{FirstName} {LastName}";
}
