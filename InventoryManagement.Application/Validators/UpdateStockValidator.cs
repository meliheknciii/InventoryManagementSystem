using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Validators;

public class UpdateStockValidator : IValidator<UpdateStockDto>
{
    public ValidationResult Validate(UpdateStockDto instance)
    {
        var result = new ValidationResult();

        result.AddErrorIf(instance.Quantity < 0, "Stock quantity cannot be negative.");

        return result;
    }
}
