using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class Product: CreateUpdateProductModel
{
    [Key]
    public Guid Id { get; set; }
    

}