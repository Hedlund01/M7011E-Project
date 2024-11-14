using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController : ControllerBase
{
    private readonly TagsService _tagsService;

    public TagsController(TagsService tagsService)
    {
        _tagsService = tagsService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTag([FromBody] CreateUpdateTagModel model)
    {
        await _tagsService.CreateTagAsync(model);
        return Ok();
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateTag(Guid id, [FromBody] CreateUpdateTagModel model)
    {
        await _tagsService.UpdateTagAsync(id, model);
        return Ok();
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteTag(Guid id)
    {
        await _tagsService.DeleteTagAsync(id);
        return Ok();
    }

    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetTag(Guid id)
    {
        var tag = await _tagsService.GetTagAsync(id);
        return Ok(tag);
    }
}