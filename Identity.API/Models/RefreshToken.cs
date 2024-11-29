using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Identity.API.Data;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Token { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
}