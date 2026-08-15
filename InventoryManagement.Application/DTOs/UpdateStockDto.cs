namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data required to update the quantity of an existing stock record.
/// </summary>
public class UpdateStockDto
{
    public int Quantity { get; set; }
}
