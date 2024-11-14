using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Identity.API.Constants;
using Microsoft.AspNetCore.Authorization;


namespace Catalog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController: ControllerBase
{

    private readonly ProductService _productService;

    public CatalogController(ProductService productService)
    {
        _productService = productService;
    }

    
    [Authorize(Roles = Role.Employee)]
    [HttpPost("createProduct")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateUpdateProductModel model)
    {
        await _productService.CreateProductAsync(model);
        return Ok();
    }
    
    [Authorize]
    [HttpPut("updateProduct/{productId}")]
    public async Task<IActionResult> UpdateProduct(int productId, [FromBody] CreateUpdateProductModel model)
    {
        await _productService.UpdateProductAsync(productId, model);
        return Ok();
    }
    
    [Authorize]
    [HttpDelete("deleteProduct/{productId}")]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        await _productService.DeleteProductAsync(Guid.Parse(productId));
        return Ok();
    }
    
    [Authorize]
    [HttpGet("getProduct/{productId}")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        
        var product = await _productService.GetProductAsync(Guid.Parse(productId));
        return Ok(product);
    }
    
    

}