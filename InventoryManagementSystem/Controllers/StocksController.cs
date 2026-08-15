using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockDto>>> GetAll(CancellationToken cancellationToken)
    {
        var stocks = await _stockService.GetAllAsync(cancellationToken);
        return Ok(stocks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var stock = await _stockService.GetByIdAsync(id, cancellationToken);
        return stock is null ? NotFound() : Ok(stock);
    }

    [HttpGet("by-product/{productId:int}")]
    public async Task<ActionResult<StockDto>> GetByProductId(int productId, CancellationToken cancellationToken)
    {
        var stock = await _stockService.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? NotFound() : Ok(stock);
    }

    [HttpPost]
    public async Task<ActionResult<StockDto>> Create(CreateStockDto dto, CancellationToken cancellationToken)
    {
        var created = await _stockService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StockDto>> Update(int id, UpdateStockDto dto, CancellationToken cancellationToken)
    {
        var updated = await _stockService.UpdateAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _stockService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
