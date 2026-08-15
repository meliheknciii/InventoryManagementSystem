namespace InventoryManagement.Domain.Interfaces;

/// <summary>
/// Generic repository abstraction used by the Application layer to access
/// persisted entities without depending on a specific Infrastructure implementation.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
