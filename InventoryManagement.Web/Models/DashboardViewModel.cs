namespace InventoryManagement.Web.Models
{
    // Ana sayfada (Home/Index) gösterilecek özet bilgileri taşıyan model.
    public class DashboardViewModel
    {
        public int CategoryCount { get; set; }
        public int ProductCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<Product> RecentProducts { get; set; } = new List<Product>();
        public List<Stock> CriticalStocks { get; set; } = new List<Stock>();
    }
}
