using System.ComponentModel.DataAnnotations;

namespace SalesPro.Web.ViewModels;

public sealed class SaleItemVm
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 999999)]
    public int Quantity { get; set; }
}
