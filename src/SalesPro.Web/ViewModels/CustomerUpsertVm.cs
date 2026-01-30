using System.ComponentModel.DataAnnotations;
using SalesPro.Web.Validation;

namespace SalesPro.Web.ViewModels;

public sealed class CustomerUpsertVm
{
    public int CustomerId { get; set; }

    [Required]
    [EcuadorCedula]
    public string Cedula { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = "";

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}
