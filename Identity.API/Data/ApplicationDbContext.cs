using Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.Constants;

namespace Identity.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityRole>().HasData(
            new { Id = "1", Name = Role.Admin, NormalizedName = Role.Admin.ToUpper() },
            new { Id = "2", Name = Role.Employee, NormalizedName = Role.Employee.ToUpper() },
            new { Id = "3", Name = Role.User, NormalizedName = Role.User.ToUpper() }
            );
  
    }
}
    
