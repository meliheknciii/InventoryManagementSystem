using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var created = await _categoryService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        var updated = await _categoryService.UpdateAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
