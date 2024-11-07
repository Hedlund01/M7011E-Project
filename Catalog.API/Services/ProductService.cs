using Catalog.API.Data;
using Catalog.API.Models;

namespace Catalog.API.Services;

public class ProductService
{
    private readonly CatalogDbContext _catalogDbContext;
    public ProductService(CatalogDbContext dbContext)
    {
        _catalogDbContext = dbContext;
    }


    public async Task CreateProductAsync(CreateUpdateProductModel product)
    {
        await _catalogDbContext.AddAsync<Product>(new ()
        {
            Description = product.Description,
            Price = product.Price,
            Name = product.Name
        }) ;
        await _catalogDbContext.SaveChangesAsync();
    }
}