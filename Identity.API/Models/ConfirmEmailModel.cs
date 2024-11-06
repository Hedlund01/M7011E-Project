namespace Identity.API.Models;

public class ConfirmEmailModel
{
    public string Email { get; set; }
    public string Token { get; set; }
}