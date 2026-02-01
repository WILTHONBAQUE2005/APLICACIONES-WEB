
using System.ComponentModel.DataAnnotations;
using SistemaPedidos.Models.Enums;

namespace SistemaPedidos.Models;

public class Payment
{
    public int PaymentId { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [Range(0.01, 99999999.99)]
    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    [StringLength(60)]
    public string? Reference { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
}
