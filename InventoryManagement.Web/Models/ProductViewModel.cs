namespace InventoryManagement.Web.Models
{
    /// <summary>
    /// Represents a product as displayed in list/detail views. Mirrors the API's ProductDto.
    /// </summary>
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
