namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data required to create a new category.
/// </summary>
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
