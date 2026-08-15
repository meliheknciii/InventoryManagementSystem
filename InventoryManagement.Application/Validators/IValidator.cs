namespace InventoryManagement.Application.Validators;

/// <summary>
/// Defines a lightweight, dependency-free validation contract for DTOs.
/// </summary>
public interface IValidator<in T>
{
    ValidationResult Validate(T instance);
}
