namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Represents a category as displayed in list/detail views. Mirrors the API's CategoryDto.
    /// </summary>
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
