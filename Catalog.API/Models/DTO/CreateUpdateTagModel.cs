using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class CreateUpdateTagModel
{
        
        [Required]
        public string Name { get; set; }

    
}