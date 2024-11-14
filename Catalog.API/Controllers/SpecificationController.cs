using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SpecificationsController : ControllerBase
{
    private readonly SpecificationsService _specificationsService;

    public SpecificationsController(SpecificationsService specificationsService)
    {
        _specificationsService = specificationsService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateSpecifications([FromBody] CreateUpdateSpecificationModel model)
    {
        await _specificationsService.CreateSpecificationAsync(model);
        return Ok();
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateSpecifications(Guid id, [FromBody] CreateUpdateSpecificationModel model)
    {
        await _specificationsService.UpdateSpecificationAsync(id, model);
        return Ok();
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteSpecifications(Guid id)
    {
        await _specificationsService.DeleteSpecificationAsync(id);
        return Ok();
    }

    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetSpecifications(Guid id)
    {
        var specifications = await _specificationsService.GetSpecificationAsync(id);
        return Ok(specifications);
    }
}