using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class CatalogDbContext(DbContextOptions options): DbContext(options)
{
    private DbSet<Product> Products { get; init; }
}