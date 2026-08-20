using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Repositories
{
    // Ürün tablosu için veritabanı işlemlerini tanımlayan arayüz.
    public interface IProductRepository
    {
        // Kategori ve stok bilgisiyle beraber (Include) tüm ürünleri getirir.
        List<Product> GetAll();

        // Ürün adı / SKU'ya göre arama ve kategoriye göre filtreleme yapar.
        // Parametreler null/boş ise ilgili filtre uygulanmaz.
        // pageNumber ve pageSize, sayfalama (pagination) için kullanılır.
        List<Product> Search(string? searchTerm, int? categoryId, int pageNumber, int pageSize);

        // Sayfalama yaparken toplam kaç sonuç olduğunu bilmemiz lazım
        // (kaç sayfa olacağını hesaplamak için). Bu yüzden ayrı bir metot yazdık.
        int SearchCount(string? searchTerm, int? categoryId);
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        bool SkuExists(string sku, int? excludeId = null);
    }
}
