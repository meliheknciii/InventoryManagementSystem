using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Interfaces;

public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}
