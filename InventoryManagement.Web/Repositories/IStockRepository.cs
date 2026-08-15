using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Repositories
{
    // Stok tablosu için veritabanı işlemlerini tanımlayan arayüz.
    public interface IStockRepository
    {
        List<Stock> GetAll();
        Stock? GetById(int id);
        Stock? GetByProductId(int productId);
        void Add(Stock stock);
        void Update(Stock stock);
        void Delete(Stock stock);
    }
}
