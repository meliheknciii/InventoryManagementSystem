using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(InventoryManagementDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(c => c.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
