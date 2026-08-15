namespace InventoryManagement.Domain.Entities;

/// <summary>
/// Represents a sellable product tracked in the inventory system.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public Stock? Stock { get; set; }
}
