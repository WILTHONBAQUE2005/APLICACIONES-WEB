using SalesPro.Application.Dtos;

namespace SalesPro.Application.Services;

public interface ISalesService
{
    Task<int> CreateSaleAsync(SaleCreateDto dto, CancellationToken ct = default);
    Task<SaleDetailsDto?> GetSaleDetailsAsync(int saleId, CancellationToken ct = default);
}
