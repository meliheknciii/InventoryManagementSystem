using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;

namespace InventoryManagement.Tests.Fakes
{
    // Kategori repo'sunun basit sahte (fake) versiyonu.
    public class FakeCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new List<Category>();
        private int _nextId = 1;

        public List<Category> GetAll()
        {
            return _categories;
        }

        public Category? GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            category.Id = _nextId;
            _nextId++;
            _categories.Add(category);
        }

        public void Update(Category category)
        {
        }

        public void Delete(Category category)
        {
            _categories.Remove(category);
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            return _categories.Any(c => c.Name == name && c.Id != excludeId);
        }
    }
}
