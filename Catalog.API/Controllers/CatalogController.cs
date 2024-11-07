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
    
}