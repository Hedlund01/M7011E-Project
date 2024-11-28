using BackgroundService.Console.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;

namespace BackgroundService.Console.Services;

public class MailService : IMailService
{
    private readonly SmtpSettings _smtpSettings;
    public MailService(SmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }

    public async Task SendEmailAsync(string toEmail, string body, string subject)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("M7011E", "m7011e@ltu.lol"));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;
        
        message.Body = new TextPart("plain")
        {
            Text = body
        };
        
        using var client = new SmtpClient();
        {
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, false);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }
}