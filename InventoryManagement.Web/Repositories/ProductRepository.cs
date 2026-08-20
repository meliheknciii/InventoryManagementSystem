using InventoryManagement.Web.Data;
using InventoryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            // Include ile ilişkili Category ve Stock tablolarını da beraber çekiyoruz,
            // aksi halde Category ve Stock alanları null gelir.
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public List<Product> Search(string? searchTerm, int? categoryId, int pageNumber, int pageSize)
        {
            var query = BuildSearchQuery(searchTerm, categoryId);

            return query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int SearchCount(string? searchTerm, int? categoryId)
        {
            return BuildSearchQuery(searchTerm, categoryId).Count();
        }

        // Search ve SearchCount metotlarının ikisi de aynı filtreleri kullandığı için
        // filtreleme kodunu buraya alıp tekrar tekrar yazmaktan kurtulduk.
        private IQueryable<Product> BuildSearchQuery(string? searchTerm, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(p =>
                    p.Name.Contains(term) ||
                    p.Sku.Contains(term));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return query;
        }

        public Product? GetById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public bool SkuExists(string sku, int? excludeId = null)
        {
            return _context.Products
                .Any(p => p.Sku == sku && p.Id != excludeId);
        }
    }
}
