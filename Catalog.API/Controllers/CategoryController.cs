using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateUpdateCategoryModel model)
    {
        await _categoryService.CreateCategoryAsync(model);
        return Ok();
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateUpdateCategoryModel model)
    {
        await _categoryService.UpdateCategoryAsync(id, model);
        return Ok();
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok();
    }

    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _categoryService.GetCategoryAsync(id);
        return Ok(category);
    }
}