using System.ComponentModel.DataAnnotations;

namespace NetCoreApi.Models;

public class FacturaDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = DateTime.Today;

    public string Moneda { get; set; } = "USD";
    public string FormaPago { get; set; } = "Efectivo";

    public EmpresaDto Empresa { get; set; } = new();
    public ClienteDto Cliente { get; set; } = new();
    public List<FacturaDetalleDto> Detalles { get; set; } = new();

    public TotalesDto Totales { get; set; } = new();

    public string Observaciones { get; set; } = "Gracias por su compra.";
}

public class EmpresaDto
{
    public string Nombre { get; set; } = "Comercial";
    public string Direccion { get; set; } = "—";
    public string Telefono { get; set; } = "—";
    public string Email { get; set; } = "—";
}

public class ClienteDto
{
    public int? Id { get; set; }
    public string Nombres { get; set; } = "Consumidor final";
    public string Direccion { get; set; } = "—";
    public string Telefono { get; set; } = "—";
    public string Email { get; set; } = "—";
}

public class FacturaDetalleDto
{
    public string Sku { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Cantidad { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrecioUnitario { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Importe { get; set; }
}

public class TotalesDto
{
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public decimal TasaImpuesto { get; set; }
}
