using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
