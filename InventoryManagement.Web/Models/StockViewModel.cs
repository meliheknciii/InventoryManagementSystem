namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Mirrors the API's StockStatus enum (Domain.Enums.StockStatus) for display purposes.
    /// </summary>
    public enum StockStatus
    {
        OutOfStock = 0,
        LowStock = 1,
        InStock = 2
    }

    /// <summary>
    /// Represents stock information as displayed in list/detail views. Mirrors the API's StockDto.
    /// </summary>
    public class StockViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public StockStatus Status { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
    }
}
