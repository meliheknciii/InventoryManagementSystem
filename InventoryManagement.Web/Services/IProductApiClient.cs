using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public interface IProductApiClient
    {
        Task<List<ProductViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductViewModel> CreateAsync(ProductFormViewModel model, CancellationToken cancellationToken = default);
        Task<ProductViewModel> UpdateAsync(int id, ProductFormViewModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
