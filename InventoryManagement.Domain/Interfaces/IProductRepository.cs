using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default);
}
