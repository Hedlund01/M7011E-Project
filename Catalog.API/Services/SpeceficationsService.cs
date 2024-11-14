using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Catalog.API.Services;

public class SpecificationsService
{
    private readonly CatalogDbContext _catalogDbContext;
    public SpecificationsService(CatalogDbContext dbContext)
    {
        _catalogDbContext = dbContext;
        
    }

    public async Task CreateSpecificationAsync(CreateUpdateSpecificationModel model)
    {
        await _catalogDbContext.AddAsync<Specifications>(new ()
        {
            Height = model.Height,
            Width = model.Width
        }) ;
        await _catalogDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateSpecificationAsync(Guid id, CreateUpdateSpecificationModel model)
    {
        var specifications = await _catalogDbContext.FindAsync<Specifications>(id);
        if (specifications == null)
        {
            throw new KeyNotFoundException("Specification not found");
        }
        
        specifications.Height = model.Height;
        specifications.Width = model.Width;

        await _catalogDbContext.SaveChangesAsync();
    }

    public async Task DeleteSpecificationAsync(Guid id)
    {
        var specifications = await _catalogDbContext.FindAsync<Specifications>(id);
        if (specifications == null)
        {
            throw new KeyNotFoundException("Specefications not found");
        }

        _catalogDbContext.Remove(specifications);
        await _catalogDbContext.SaveChangesAsync();
    }


    public async Task<Specifications?> GetSpecificationAsync(Guid id)
    {
        return await _catalogDbContext.FindAsync<Specifications>(id);
    }
}