using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class CreateFullProductModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public float Price { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public CreateUpdateCategoryModel Category { get; set; }
    
    [Required]
    public ICollection<CreateUpdateTagModel> Tags { get; set; }
    
    [Required]
    public CreateUpdateSpecificationModel Specification { get; set; }
}