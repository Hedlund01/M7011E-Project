using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class Category
{
    [Key]
    public Guid Id { get; init; }
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }

    public ICollection<Product> Products { get; } = new List<Product>();
}