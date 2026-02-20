using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreApi.Data;
using NetCoreApi.Models;

namespace NetCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly AppDbContext _db;

    public FacturasController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /api/Facturas/demo?clienteId=1
    [HttpGet("demo")]
    public async Task<ActionResult<FacturaDto>> Demo([FromQuery] int? clienteId, CancellationToken ct)
    {
        var cliente = await ResolveCliente(clienteId, ct);
        var factura = BuildFactura(id: 1, cliente: cliente);

        return Ok(factura);
    }

    // GET: /api/Facturas/5?clienteId=1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FacturaDto>> GetById(int id, [FromQuery] int? clienteId, CancellationToken ct)
    {
        var cliente = await ResolveCliente(clienteId, ct);
        var factura = BuildFactura(id: id, cliente: cliente);

        return Ok(factura);
    }

    private async Task<ClienteDto> ResolveCliente(int? clienteId, CancellationToken ct)
    {
        ClienteModel? c;

        if (clienteId.HasValue)
        {
            c = await _db.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == clienteId.Value, ct);
        }
        else
        {
            c = await _db.Clientes
                .AsNoTracking()
                .OrderBy(x => x.id)
                .FirstOrDefaultAsync(ct);
        }

        if (c is null)
        {
            return new ClienteDto
            {
                Id = null,
                Nombres = "Consumidor final",
                Direccion = "—",
                Telefono = "—",
                Email = "—"
            };
        }

        return new ClienteDto
        {
            Id = c.id,
            Nombres = c.Nombres ?? "Cliente",
            Direccion = c.Direccion ?? "—",
            Telefono = c.Telefono ?? "—",
            Email = c.Email ?? "—"
        };
    }

    private static FacturaDto BuildFactura(int id, ClienteDto cliente)
    {
        var empresa = new EmpresaDto
        {
            Nombre = "Comercial",
            Direccion = "Av. Principal y Calle Secundaria",
            Telefono = "099 000 0000",
            Email = "ventas@comercial.com"
        };

        var detalles = new List<FacturaDetalleDto>
        {
            new()
            {
                Sku = "SKU-001",
                Descripcion = "Servicio técnico",
                Cantidad = 1m,
                PrecioUnitario = 25.00m
            },
            new()
            {
                Sku = "SKU-002",
                Descripcion = "Repuesto / accesorio",
                Cantidad = 2m,
                PrecioUnitario = 7.50m
            },
            new()
            {
                Sku = "SKU-003",
                Descripcion = "Mano de obra",
                Cantidad = 1m,
                PrecioUnitario = 12.00m
            }
        };

        foreach (var d in detalles)
            d.Importe = Math.Round(d.Cantidad * d.PrecioUnitario, 2);

        var subtotal = detalles.Sum(x => x.Importe);
        var tasa = 0.12m;
        var impuesto = Math.Round(subtotal * tasa, 2);
        var total = subtotal + impuesto;

        return new FacturaDto
        {
            Id = id,
            Numero = $"FAC-{id:000000}",
            FechaEmision = DateTime.Today,
            Moneda = "USD",
            FormaPago = "Efectivo",
            Empresa = empresa,
            Cliente = cliente,
            Detalles = detalles,
            Totales = new TotalesDto
            {
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total,
                TasaImpuesto = tasa
            },
            Observaciones = "Documento generado para fines de reporte."
        };
    }
}
