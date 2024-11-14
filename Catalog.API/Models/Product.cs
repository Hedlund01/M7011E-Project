using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public float Price { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }
    
    public Category Category { get; set; }
    
    [ForeignKey(nameof(Specification))]
    public Guid SpecificationId { get; set; }
    
    public Specifications Specification { get; set; }


    public ICollection<Tags> Tags { get; init; } 


}