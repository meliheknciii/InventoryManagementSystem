using InventoryManagement.Web.Data;
using InventoryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Stock> GetAll()
        {
            return _context.Stocks
                .Include(s => s.Product)
                .OrderBy(s => s.Quantity)
                .ToList();
        }

        public Stock? GetById(int id)
        {
            return _context.Stocks
                .Include(s => s.Product)
                .FirstOrDefault(s => s.Id == id);
        }

        public Stock? GetByProductId(int productId)
        {
            return _context.Stocks
                .Include(s => s.Product)
                .FirstOrDefault(s => s.ProductId == productId);
        }

        public void Add(Stock stock)
        {
            _context.Stocks.Add(stock);
            _context.SaveChanges();
        }

        public void Update(Stock stock)
        {
            _context.Stocks.Update(stock);
            _context.SaveChanges();
        }

        public void Delete(Stock stock)
        {
            _context.Stocks.Remove(stock);
            _context.SaveChanges();
        }
    }
}
