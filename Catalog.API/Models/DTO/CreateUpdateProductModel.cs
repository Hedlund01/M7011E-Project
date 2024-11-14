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
    
    [Required]
    public Guid CategoryId { get; set; }
    
    [Required]
    public ICollection<Guid> TagIds { get; set; }
    
    [Required]
    public Guid SpecificationId { get; set; }
    
}