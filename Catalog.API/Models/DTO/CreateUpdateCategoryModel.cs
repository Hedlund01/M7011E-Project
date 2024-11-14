using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class CreateUpdateCategoryModel
{
            
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }
    
}