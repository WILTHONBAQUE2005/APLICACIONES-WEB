namespace SalesPro.Web.ViewModels;

public sealed class PagerVm
{
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public string BaseUrl { get; init; } = "";
}
