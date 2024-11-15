using Microsoft.AspNetCore.Mvc;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Constants;

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

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPost("create")]
    public async Task<ApiResponse<Specifications>> CreateSpecifications([FromBody] CreateUpdateSpecificationModel model)
    {
        try
        {
            var obj = await _specificationsService.CreateSpecificationAsync(model);
            return new ApiResponse<Specifications> { Success = true, Data = obj};
        }
        catch (Exception e)
        {
            return new ApiResponse<Specifications>(){ Success = false, Message = "An error has occured" };
        }
        
    }

    [Authorize(Roles = $"{Role.Admin},{Role.Employee}")]
    [HttpPut("update/{id}")]
    public async Task<ApiResponse> UpdateSpecifications(Guid id, [FromBody] CreateUpdateSpecificationModel model)
    {
        try
        {
            await _specificationsService.UpdateSpecificationAsync(id, model);
            return new ApiResponse { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }

    [Authorize(Roles = Role.Admin)]
    [HttpDelete("delete/{id}")]
    public async Task<ApiResponse> DeleteSpecifications(Guid id)
    {
        try
        {
            await _specificationsService.DeleteSpecificationAsync(id);
            return new ApiResponse { Success = true };
        }
        catch (Exception e)
        {
            return new ApiResponse(){ Success = false, Message = "An error has occured" };
        }
        
    }

    [HttpGet("get/{id}")]
    public async Task<ApiResponse<Specifications>> GetSpecifications(Guid id)
    {
        try
        {
            var specifications = await _specificationsService.GetSpecificationAsync(id);
            return new ApiResponse<Specifications> { Success = true, Data= specifications };
        }
        catch (Exception e)
        {
            return new ApiResponse<Specifications>(){ Success = false, Message = "An error has occured" };
        }
        
    }
    
    [HttpGet("get")]
    public async Task<ApiResponse<PaginatedList<Specifications>>> GetSpecifications(int pageIndex, int pageSize)
    {
        try
        {
            var list = await _specificationsService.GetSpecificationsAsync(pageIndex, pageSize);

            if (list.Count <= 0)
            {
                return new ApiResponse<PaginatedList<Specifications>> { Success = false, Message = "No objects found" };
            }
            var paginated = new PaginatedList<Specifications>
            {
                Items = list,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling(list.Count / (double)pageSize)
            };
            return new ApiResponse<PaginatedList<Specifications>> { Success = true, Data = paginated };
        }
        catch (Exception e)
        {
            return new ApiResponse<PaginatedList<Specifications>>(){ Success = false, Message = "An error has occured" };
        }
        
    }
}