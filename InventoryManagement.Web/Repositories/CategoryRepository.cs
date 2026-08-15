using InventoryManagement.Web.Data;
using InventoryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Repositories
{
    // ICategoryRepository arayüzünün somut (gerçek) implementasyonu.
    // Veritabanı işlemlerini burada EF Core ile senkron olarak yapıyoruz.
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAll()
        {
            return _context.Categories
                .OrderBy(c => c.Name)
                .ToList();
        }

        public Category? GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            return _context.Categories
                .Any(c => c.Name == name && c.Id != excludeId);
        }
    }
}
