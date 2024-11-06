using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Token { get; set; }
    
    [Required]
    public string UserId { get; set; }
    [Required]
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
}