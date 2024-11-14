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

    public async Task CreateProductAsync(CreateUpdateProductModel model)
    {
        var product = _mapper.Map<Product>(model);
        await _catalogDbContext.AddAsync(product);
        await _catalogDbContext.SaveChangesAsync();
    }
    
    public async Task CreateProductFullAsync(CreateFullProductModel model)
    {
        List<Guid> tags = [];
        foreach (var tag in model.Tags)
        {
            var result = await _catalogDbContext.AddAsync(new Tags()
            {
                Name = tag.Name
            });
            
            tags.Add(result.Entity.Id);
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
            CategoryId = cat.Entity.Id,
            SpecificationId = spec.Entity.Id,

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
}