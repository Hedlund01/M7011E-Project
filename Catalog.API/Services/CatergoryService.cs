using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Catalog.API.Services;

public class CategoryService
{
    private readonly CatalogDbContext _catalogDbContext;
    private readonly IMapper _mapper;
    public CategoryService(CatalogDbContext dbContext, IMapper mapper)
    {
        _catalogDbContext = dbContext;
        _mapper = mapper;
    }

    public async Task CreateCategoryAsync(CreateUpdateCategoryModel model)
    {
        var tag = _mapper.Map<Category>(model);
        await _catalogDbContext.AddAsync(tag);
        await _catalogDbContext.SaveChangesAsync();
    }
    
    
    public async Task UpdateCategoryAsync(Guid id, CreateUpdateCategoryModel model)
    {
        var category = await _catalogDbContext.FindAsync<Category>(id);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }
        
        category.Name = model.Name;
        category.Description = model.Description;

        await _catalogDbContext.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _catalogDbContext.FindAsync<Category>(id);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        _catalogDbContext.Remove(category);
        await _catalogDbContext.SaveChangesAsync();
    }


    public async Task<Category?> GetCategoryAsync(Guid id)
    {
        return await _catalogDbContext.FindAsync<Category>(id);
    }
}