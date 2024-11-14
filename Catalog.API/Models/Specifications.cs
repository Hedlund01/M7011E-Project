using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models;

public class Specifications
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Height { get; set; }
    
    [Required]
    public string Width { get; set; }
    

    
}