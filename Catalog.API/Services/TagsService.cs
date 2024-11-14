using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Catalog.API.Services;

public class TagsService
{
    private readonly CatalogDbContext _catalogDbContext;
    public TagsService(CatalogDbContext dbContext)
    {
        _catalogDbContext = dbContext;
        
    }

    public async Task CreateTagAsync(CreateUpdateTagModel model)
    {
        await _catalogDbContext.AddAsync<Tags>(new ()
        {
            Name = model.Name
        }) ;
        await _catalogDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateTagAsync(Guid id, CreateUpdateTagModel model)
    {
        var tag = await _catalogDbContext.FindAsync<Tags>(id);
        if (tag == null)
        {
            throw new KeyNotFoundException("Tags not found");
        }
        
        tag.Name = model.Name;

        await _catalogDbContext.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(Guid id)
    {
        var product = await _catalogDbContext.FindAsync<Tags>(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Tags not found");
        }

        _catalogDbContext.Remove(product);
        await _catalogDbContext.SaveChangesAsync();
    }


    public async Task<Tags?> GetTagAsync(Guid id)
    {
        return await _catalogDbContext.FindAsync<Tags>(id);
    }
}