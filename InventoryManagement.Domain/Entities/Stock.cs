using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents the stock quantity information for a single product.
/// </summary>
public class Stock
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }
    public StockStatus Status { get; set; } = StockStatus.OutOfStock;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
