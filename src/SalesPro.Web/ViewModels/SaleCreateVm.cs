using System.ComponentModel.DataAnnotations;

namespace SalesPro.Web.ViewModels;

public sealed class SaleCreateVm
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(typeof(decimal), "0", "1")]
    public decimal VatRate { get; set; }

    [MinLength(1)]
    public List<SaleItemVm> Items { get; set; } = new();
}
