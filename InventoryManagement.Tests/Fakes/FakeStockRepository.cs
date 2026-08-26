using InventoryManagement.Web.Models;
using InventoryManagement.Web.Repositories;

namespace InventoryManagement.Tests.Fakes
{
    // Gerçek veritabanı yerine liste üzerinde çalışan basit sahte repo.
    // Moq gibi bir kütüphane kullanmak yerine kendimiz yazdık.
    public class FakeStockRepository : IStockRepository
    {
        private readonly List<Stock> _stocks = new List<Stock>();
        private int _nextId = 1;

        public List<Stock> GetAll()
        {
            return _stocks;
        }

        public Stock? GetById(int id)
        {
            return _stocks.FirstOrDefault(s => s.Id == id);
        }

        public Stock? GetByProductId(int productId)
        {
            return _stocks.FirstOrDefault(s => s.ProductId == productId);
        }

        public void Add(Stock stock)
        {
            stock.Id = _nextId;
            _nextId++;
            _stocks.Add(stock);
        }

        public void Update(Stock stock)
        {
            // liste zaten aynı referansı tuttuğu için ekstra bir şey yapmaya gerek yok
        }

        public void Delete(Stock stock)
        {
            _stocks.Remove(stock);
        }
    }
}
