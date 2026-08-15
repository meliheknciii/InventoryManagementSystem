using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(InventoryManagementDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet
            .AsNoTracking()
            .Include(s => s.Product)
            .ToListAsync(cancellationToken);

    public async Task<Stock?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);
}
