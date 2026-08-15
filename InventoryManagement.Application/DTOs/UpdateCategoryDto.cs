namespace InventoryManagement.Application.DTOs;

/// <summary>
/// Data required to update an existing category.
/// </summary>
public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
