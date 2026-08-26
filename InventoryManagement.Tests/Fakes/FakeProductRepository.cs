using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;

namespace InventoryManagement.Tests.Fakes
{
    // Ürün repo'sunun basit sahte (fake) versiyonu.
    public class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new List<Product>();
        private int _nextId = 1;

        public List<Product> GetAll()
        {
            return _products;
        }

        public List<Product> Search(string? searchTerm, int? categoryId, int pageNumber, int pageSize)
        {
            // Testlerde arama/sayfalama kullanmıyoruz, basitçe hepsini döndürüyoruz.
            return _products;
        }

        public int SearchCount(string? searchTerm, int? categoryId)
        {
            return _products.Count;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            product.Id = _nextId;
            _nextId++;
            _products.Add(product);
        }

        public void Update(Product product)
        {
        }

        public void Delete(Product product)
        {
            _products.Remove(product);
        }

        public bool SkuExists(string sku, int? excludeId = null)
        {
            return _products.Any(p => p.Sku == sku && p.Id != excludeId);
        }
    }
}
