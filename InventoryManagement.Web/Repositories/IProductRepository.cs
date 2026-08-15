using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Repositories
{
    // Ürün tablosu için veritabanı işlemlerini tanımlayan arayüz.
    public interface IProductRepository
    {
        // Kategori ve stok bilgisiyle beraber (Include) tüm ürünleri getirir.
        List<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        bool SkuExists(string sku, int? excludeId = null);
    }
}
