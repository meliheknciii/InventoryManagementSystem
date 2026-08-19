using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
        bool NameExists(string name, int? excludeId = null);
    }
}
