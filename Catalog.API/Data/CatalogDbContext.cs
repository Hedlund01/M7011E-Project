using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Specifications> Specifications { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Category> Categories { get; set; }
}