using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Constants;

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

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPost("create")]
    public async Task<ApiResponse<Category>> CreateCategory([FromBody] CreateUpdateCategoryModel model)
    {
        try
        {
            var obj = await _categoryService.CreateCategoryAsync(model);
            return new ApiResponse<Category>() { Success = true, Data = obj};
        }
        catch (Exception e)
        {
            return new ApiResponse<Category>(){ Success = false, Message = "An error has occured" };
        }
       
    }

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPut("update/{id}")]
    public async Task<ApiResponse> UpdateCategory(Guid id, [FromBody] CreateUpdateCategoryModel model)
    {
        try
        {
            await _categoryService.UpdateCategoryAsync(id, model);
            return new ApiResponse() { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
       
    }

    [Authorize(Roles = Role.Admin)]
    [HttpDelete("delete/{id}")]
    public async Task<ApiResponse> DeleteCategory(Guid id)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id);
            return new ApiResponse { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }

    [HttpGet("get/{id}")]
    public async Task<ApiResponse<Category>> GetCategory(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryAsync(id);
            return new ApiResponse<Category>() { Success = true, Data = category };
        }
        catch (Exception e)
        {
            return new ApiResponse<Category>() { Success = false, Message = "An error has occured" };
        }
    }
    
    [HttpGet("get")]
    public async Task<ApiResponse<PaginatedList<Category>>> GetCategories(int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesAsync(pageIndex, pageSize);
            if (categories.Count <= 0)
            {
                return new ApiResponse<PaginatedList<Category>> { Success = false, Message = "No objects found" };
            }
            var paginatedList = new PaginatedList<Category>
            {
                Items = categories,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling(categories.Count / (double)pageSize)
            };
            return new ApiResponse<PaginatedList<Category>> { Success = true, Data = paginatedList };
        }
        catch (Exception e)
        {
            return new ApiResponse<PaginatedList<Category>> { Success = false, Message = "An error has occured" };
        }
    }
}