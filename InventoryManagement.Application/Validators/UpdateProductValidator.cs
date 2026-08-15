using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Validators;

public class UpdateProductValidator : IValidator<UpdateProductDto>
{
    public ValidationResult Validate(UpdateProductDto instance)
    {
        var result = new ValidationResult();

        result.AddErrorIf(string.IsNullOrWhiteSpace(instance.Name), "Product name is required.");
        result.AddErrorIf(instance.Name?.Length > 200, "Product name must not exceed 200 characters.");
        result.AddErrorIf(string.IsNullOrWhiteSpace(instance.Sku), "Product SKU is required.");
        result.AddErrorIf(instance.Sku?.Length > 50, "Product SKU must not exceed 50 characters.");
        result.AddErrorIf(instance.Price <= 0, "Product price must be greater than zero.");
        result.AddErrorIf(instance.CategoryId <= 0, "A valid category must be selected.");

        return result;
    }
}
