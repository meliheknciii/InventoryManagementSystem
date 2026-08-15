namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data required to create a stock record for a product.
/// </summary>
public class CreateStockDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
