using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Constants;

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

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPost("create")]
    public async Task<ApiResponse<Tags>> CreateTag([FromBody] CreateUpdateTagModel model)
    {
        try
        {
            var tag = await _tagsService.CreateTagAsync(model);
            return new ApiResponse<Tags> { Success = true, Data = tag };
        }
        catch (Exception e)
        {
            return new ApiResponse<Tags> { Success = false, Message = "An error has occured" };
        }
    }

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPut("update/{id}")]
    public async Task<ApiResponse> UpdateTag(Guid id, [FromBody] CreateUpdateTagModel model)
    {
        try
        {
            await _tagsService.UpdateTagAsync(id, model);
            return new ApiResponse() { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse() { Success = false, Message = "An error has occured" };
        }
        
    }

    [Authorize(Roles = Role.Admin)]
    [HttpDelete("delete/{id}")]
    public async Task<ApiResponse> DeleteTag(Guid id)
    {
        try
        {
            await _tagsService.DeleteTagAsync(id);   
            return new ApiResponse() { Success = true };
        }catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }

    [HttpGet("get/{id}")]
    public async Task<ApiResponse> GetTag(Guid id)
    {
        try
        {
            var tag = await _tagsService.GetTagAsync(id);
            return new ApiResponse<Tags> { Success = true , Data = tag };
        }
        catch (Exception e)
        {
            return new ApiResponse<Tags>{ Success = false, Message = "An error has occured" };
        }
        
        
    }

    [HttpGet("get")]
    public async Task<ApiResponse<PaginatedList<Tags>>> GetTags(int pageIndex, int pageSize)
    {
        try
        {
            var tags = await _tagsService.GetTagsAsync(pageIndex, pageSize);

            if (tags.Count <= 0)
            {
                return new ApiResponse<PaginatedList<Tags>> { Success = false, Message = "No objects found" };
            } 
            var paginatedTags = new PaginatedList<Tags>
            {
                Items = tags,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling(tags.Count / (double)pageSize)
            };
            return new ApiResponse<PaginatedList<Tags>> { Success = true, Data = paginatedTags };
        }
        catch (Exception e)
        {
            return new ApiResponse<PaginatedList<Tags>> { Success = false, Message = "An error has occured" };
        }
        
    }
}