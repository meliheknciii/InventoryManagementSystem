using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Validators;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services;

/// <summary>
/// Implements stock business operations on top of the repository abstraction.
/// </summary>
public class StockService : IStockService
{
    private const int LowStockThreshold = 10;

    private readonly IStockRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateStockDto> _createValidator;
    private readonly IValidator<UpdateStockDto> _updateValidator;

    public StockService(
        IStockRepository stockRepository,
        IProductRepository productRepository,
        IValidator<CreateStockDto> createValidator,
        IValidator<UpdateStockDto> updateValidator)
    {
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stocks = await _stockRepository.GetAllAsync(cancellationToken);
        return stocks.Select(MapToDto).ToList();
    }

    public async Task<StockDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByIdAsync(id, cancellationToken);
        return stock is null ? null : MapToDto(stock);
    }

    public async Task<StockDto?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? null : MapToDto(stock);
    }

    public async Task<StockDto> CreateAsync(CreateStockDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _createValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.ProductId} was not found.");

        var existingStock = await _stockRepository.GetByProductIdAsync(dto.ProductId, cancellationToken);
        if (existingStock is not null)
        {
            throw new InvalidOperationException($"Product with id {dto.ProductId} already has a stock record.");
        }

        var stock = new Stock
        {
            ProductId = dto.ProductId,
            Product = product,
            Quantity = dto.Quantity,
            Status = CalculateStatus(dto.Quantity),
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _stockRepository.AddAsync(stock, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(stock);
    }

    public async Task<StockDto> UpdateAsync(int id, UpdateStockDto dto, CancellationToken cancellationToken = default)
    {
        var validation = _updateValidator.Validate(dto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        var stock = await _stockRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Stock record with id {id} was not found.");

        stock.Quantity = dto.Quantity;
        stock.Status = CalculateStatus(dto.Quantity);
        stock.LastUpdatedUtc = DateTime.UtcNow;

        _stockRepository.Update(stock);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(stock);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Stock record with id {id} was not found.");

        _stockRepository.Remove(stock);
        await _stockRepository.SaveChangesAsync(cancellationToken);
    }

    private static StockStatus CalculateStatus(int quantity) => quantity switch
    {
        <= 0 => StockStatus.OutOfStock,
        <= LowStockThreshold => StockStatus.LowStock,
        _ => StockStatus.InStock
    };

    private static StockDto MapToDto(Stock stock) => new()
    {
        Id = stock.Id,
        ProductId = stock.ProductId,
        ProductName = stock.Product?.Name,
        Quantity = stock.Quantity,
        Status = stock.Status,
        LastUpdatedUtc = stock.LastUpdatedUtc
    };
}
