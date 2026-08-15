using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Validators;

public class UpdateCategoryValidator : IValidator<UpdateCategoryDto>
{
    public ValidationResult Validate(UpdateCategoryDto instance)
    {
        var result = new ValidationResult();

        result.AddErrorIf(string.IsNullOrWhiteSpace(instance.Name), "Category name is required.");
        result.AddErrorIf(instance.Name?.Length > 100, "Category name must not exceed 100 characters.");
        result.AddErrorIf(instance.Description?.Length > 500, "Category description must not exceed 500 characters.");

        return result;
    }
}
