namespace BackgroundService.Console.Services;

public interface IMailService
{
    Task SendEmailAsync(string toEmail, string body, string subject);
}