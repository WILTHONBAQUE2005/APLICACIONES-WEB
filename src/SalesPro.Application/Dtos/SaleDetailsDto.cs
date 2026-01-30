namespace SalesPro.Application.Dtos;

public sealed class SaleDetailsDto
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public DateTime SaleDate { get; set; }

    public int CustomerId { get; set; }
    public string CustomerCedula { get; set; } = null!;
    public string CustomerFullName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public string? CustomerPhone { get; set; }

    public decimal Subtotal { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal Total { get; set; }

    public List<SaleDetailsItemDto> Items { get; set; } = new();
}
