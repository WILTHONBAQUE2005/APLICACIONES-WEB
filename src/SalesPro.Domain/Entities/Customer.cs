namespace SalesPro.Domain.Entities;

public sealed class Customer
{
    public int CustomerId { get; set; }
    public string Cedula { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Sale> Sales { get; set; } = new();
}
