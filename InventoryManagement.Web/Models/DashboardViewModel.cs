namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Aggregated data shown on the dashboard (Home/Index).
    /// </summary>
    public class DashboardViewModel
    {
        public int CategoryCount { get; set; }
        public int ProductCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<ProductViewModel> RecentProducts { get; set; } = new();
        public List<StockViewModel> CriticalStocks { get; set; } = new();
        public bool ApiUnavailable { get; set; }
    }
}
