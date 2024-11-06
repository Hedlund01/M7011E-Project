namespace Identity.API.Models;

public class TokenRequest
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
}