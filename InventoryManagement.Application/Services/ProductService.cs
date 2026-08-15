using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Validators;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services;

/// <summary>
/// Implements product business operations on top of the repository abstraction.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _createValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {dto.CategoryId} was not found.");

        if (await _productRepository.ExistsBySkuAsync(dto.Sku, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"A product with SKU '{dto.Sku}' already exists.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Sku = dto.Sku,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            Category = category
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _updateValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        var product = await _productRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {id} was not found.");

        if (dto.CategoryId != product.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with id {dto.CategoryId} was not found.");
            product.Category = category;
            product.CategoryId = dto.CategoryId;
        }

        if (await _productRepository.ExistsBySkuAsync(dto.Sku, id, cancellationToken))
        {
            throw new InvalidOperationException($"Another product with SKU '{dto.Sku}' already exists.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Sku = dto.Sku;
        product.Price = dto.Price;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {id} was not found.");

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Sku = product.Sku,
        Price = product.Price,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name
    };
}
