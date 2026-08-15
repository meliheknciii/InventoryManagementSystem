namespace InventoryManagement.Application.Validators;

/// <summary>
/// Represents the outcome of validating a DTO.
/// </summary>
public class ValidationResult
{
    public List<string> Errors { get; } = new();

    public bool IsValid => Errors.Count == 0;

    public void AddErrorIf(bool condition, string errorMessage)
    {
        if (condition)
        {
            Errors.Add(errorMessage);
        }
    }
}
