namespace SalesPro.Domain.Entities;

public sealed class Sale
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal Total { get; set; }

    public Customer Customer { get; set; } = null!;
    public List<SaleItem> Items { get; set; } = new();
}
