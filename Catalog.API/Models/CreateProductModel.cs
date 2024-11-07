using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class CreateUpdateProductModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public float Price { get; set; }
    
    [Required]
    public string Description { get; set; }
}