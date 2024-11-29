using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Constants;


namespace Catalog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController: ControllerBase
{

    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    
    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPost("create")]
    public async Task<ApiResponse<Product>> CreateProduct([FromBody] CreateUpdateProductModel model)
    {
        try
        {
            var obj = await _productService.CreateProductAsync(model);
            return new ApiResponse<Product>() { Success = true , Data = obj};
        }
        catch (Exception e)
        {
            return new ApiResponse<Product>(){ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPost("createFull")]
    public async Task<ApiResponse> CreateFullProduct([FromBody] CreateFullProductModel model)
    {
        try
        {
            var obj = await _productService.CreateProductFullAsync(model);
            return new ApiResponse() { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPut("update/{productId}")]
    public async Task<ApiResponse> UpdateProduct(int productId, [FromBody] CreateUpdateProductModel model)
    {
        try
        {
            await _productService.UpdateProductAsync(productId, model);
            return new ApiResponse() { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpDelete("delete/{productId}")]
    public async Task<ApiResponse> DeleteProduct(string productId)
    {
        try
        {
            await _productService.DeleteProductAsync(Guid.Parse(productId));
            return new ApiResponse() { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [HttpGet("get/{productId}")]
    public async Task<ApiResponse> GetProduct(string productId)
    {

        try
        {
            var product = await _productService.GetProductAsync(Guid.Parse(productId));
            return new ApiResponse<Product> { Success = true, Data = product};
        }
        catch (Exception e)
        {
            return new ApiResponse<Product>{ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [HttpGet("get")]
    public async Task<ApiResponse<PaginatedList<Product>>> GetProducts(int pageIndex, int pageSize)
    {
        try
        {
            var list = await _productService.GetProductsAsync(pageIndex, pageSize);
            if (list.Count <= 0)
            {
                return new ApiResponse<PaginatedList<Product>> { Success = false, Message = "No objects found" };
            }
            var paginated = new PaginatedList<Product>
            {
                Items = list,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize)
            };
            return new ApiResponse<PaginatedList<Product>> { Success = true, Data = paginated };
        }
        catch (Exception e)
        {
           return new ApiResponse<PaginatedList<Product>>() { Success = false, Message = "An error has occured" };
        }
       
        
    }

}