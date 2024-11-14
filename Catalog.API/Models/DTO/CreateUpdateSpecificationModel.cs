using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class CreateUpdateSpecificationModel
{
            
    [Required]
    public string Height { get; set; }
    
    [Required]
    public string Width { get; set; }
}