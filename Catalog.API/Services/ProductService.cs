using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Catalog.API.Services;

public class ProductService
{
    private readonly CatalogDbContext _catalogDbContext;
    private readonly IMapper _mapper;
    public ProductService(CatalogDbContext dbContext, IMapper mapper)
    {
        _catalogDbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Product> CreateProductAsync(CreateUpdateProductModel model)
    {
        var product = _mapper.Map<Product>(model);
        var obj = await _catalogDbContext.AddAsync(product);
        await _catalogDbContext.SaveChangesAsync();
        return obj.Entity;
    }
    
    public async Task CreateProductFullAsync(CreateFullProductModel model)
    {
        List<Tags> tags = [];
        foreach (var tag in model.Tags)
        {
            var result = await _catalogDbContext.AddAsync(new Tags()
            {
                Name = tag.Name
            });
            
            tags.Add(result.Entity);
        }

        var cat = await _catalogDbContext.AddAsync(new Category()
        {
            Name = model.Category.Name,
            Description = model.Category.Description
        });

        var spec = await _catalogDbContext.AddAsync(new Specifications()
        {
            Height = model.Specification.Height,
            Width = model.Specification.Width
        });

        await _catalogDbContext.AddAsync(new Product()
        {
            Description = model.Description,
            Tags = tags,
            Name = model.Name,
            Price = model.Price,
            Category = cat.Entity,
            Specification = spec.Entity

        });
        await _catalogDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateProductAsync(int productId, CreateUpdateProductModel updatedProduct)
    {
        var product = await _catalogDbContext.FindAsync<Product>(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.Name = updatedProduct.Name;

        await _catalogDbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        var product = await _catalogDbContext.FindAsync<Product>(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        _catalogDbContext.Remove(product);
        await _catalogDbContext.SaveChangesAsync();
    }


    public async Task<Product?> GetProductAsync(Guid id)
    {
        return await _catalogDbContext.FindAsync<Product>(id);
    }
    
    public async Task<List<Product>> GetProductsAsync(int pageIndex, int pageSize)
    {
        return await _catalogDbContext.Products.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}