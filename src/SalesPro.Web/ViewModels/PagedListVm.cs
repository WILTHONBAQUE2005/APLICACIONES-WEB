namespace SalesPro.Web.ViewModels;

public sealed class PagedListVm<T>
{
    public List<T> Items { get; init; } = new();
    public PagerVm Pager { get; init; } = new();
    public string? Query { get; init; }
}
