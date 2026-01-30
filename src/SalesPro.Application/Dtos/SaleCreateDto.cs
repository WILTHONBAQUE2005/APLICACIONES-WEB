namespace SalesPro.Application.Dtos;

public sealed class SaleCreateDto
{
    public int CustomerId { get; set; }
    public decimal VatRate { get; set; }
    public List<SaleItemCreateDto> Items { get; set; } = new();
}
