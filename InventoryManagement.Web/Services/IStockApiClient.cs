using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public interface IStockApiClient
    {
        Task<List<StockViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<StockViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<StockViewModel> CreateAsync(StockFormViewModel model, CancellationToken cancellationToken = default);
        Task<StockViewModel> UpdateAsync(int id, StockFormViewModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
