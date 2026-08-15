namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data required to create a new product.
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
