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

    public async Task<Category> CreateCategoryAsync(CreateUpdateCategoryModel model)
    {
        var data = _mapper.Map<Category>(model);
        var obj = await _catalogDbContext.AddAsync(data);
        await _catalogDbContext.SaveChangesAsync();
        return obj.Entity;
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
    
    public async Task<List<Category>> GetCategoriesAsync(int pageIndex, int pageSize)
    {
        return await _catalogDbContext.Categories.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

}