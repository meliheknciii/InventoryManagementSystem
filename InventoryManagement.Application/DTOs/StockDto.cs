using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Represents stock information for a product as exposed to API consumers.
/// </summary>
public class StockDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public StockStatus Status { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
