using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Defines the business operations available for managing product stock levels.
/// </summary>
public interface IStockService
{
    Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StockDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StockDto?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<StockDto> CreateAsync(CreateStockDto dto, CancellationToken cancellationToken = default);
    Task<StockDto> UpdateAsync(int id, UpdateStockDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
