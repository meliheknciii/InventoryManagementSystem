using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoryViewModel> CreateAsync(CategoryFormViewModel model, CancellationToken cancellationToken = default);
        Task<CategoryViewModel> UpdateAsync(int id, CategoryFormViewModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
