using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Repositories
{
    // Kategori tablosu için yapılacak veritabanı işlemlerini tanımlayan arayüz (interface).
    // Controller bu arayüze bağımlı olacak, somut sınıfa değil (Dependency Injection mantığı).
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
