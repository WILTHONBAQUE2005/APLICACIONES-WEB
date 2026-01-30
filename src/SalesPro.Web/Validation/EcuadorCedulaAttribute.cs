using System.ComponentModel.DataAnnotations;
using SalesPro.Domain.Validation;

namespace SalesPro.Web.Validation;

public sealed class EcuadorCedulaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var s = value as string;
        if (!EcuadorCedula.IsValid(s))
            return new ValidationResult("Cédula inválida.");
        return ValidationResult.Success;
    }
}
