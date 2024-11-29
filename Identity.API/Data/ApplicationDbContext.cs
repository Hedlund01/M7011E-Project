using Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.Constants;

namespace Identity.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(Role.Admin))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = Role.Admin, NormalizedName = Role.Admin.ToUpper() });
        }
        if (!await roleManager.RoleExistsAsync(Role.Employee))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = Role.Employee, NormalizedName = Role.Employee.ToUpper() });
        }
        if (!await roleManager.RoleExistsAsync(Role.User))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = Role.User, NormalizedName = Role.User.ToUpper() });
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
    
