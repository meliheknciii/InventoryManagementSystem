using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Validators;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services;

/// <summary>
/// Implements category business operations on top of the repository abstraction.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateCategoryDto> _createValidator;
    private readonly IValidator<UpdateCategoryDto> _updateValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IValidator<CreateCategoryDto> createValidator,
        IValidator<UpdateCategoryDto> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _createValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        if (await _categoryRepository.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _updateValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {id} was not found.");

        if (await _categoryRepository.ExistsByNameAsync(dto.Name, id, cancellationToken))
        {
            throw new InvalidOperationException($"Another category named '{dto.Name}' already exists.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {id} was not found.");

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }

    private static CategoryDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description
    };
}
