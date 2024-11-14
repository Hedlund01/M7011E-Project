using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models;

public class Tags
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    public ICollection<Product> Products { get; init; }
}