using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Validators;

public class CreateStockValidator : IValidator<CreateStockDto>
{
    public ValidationResult Validate(CreateStockDto instance)
    {
        var result = new ValidationResult();

        result.AddErrorIf(instance.ProductId <= 0, "A valid product must be selected.");
        result.AddErrorIf(instance.Quantity < 0, "Stock quantity cannot be negative.");

        return result;
    }
}
