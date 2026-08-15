using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces;

/// <summary>
/// Defines the business operations available for managing categories.
/// </summary>
public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
