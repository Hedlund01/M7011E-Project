using BackgroundService.Console.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Console.Services;

public class MailService : IMailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailService> _logger;
    public MailService(SmtpSettings smtpSettings, IConfiguration configuration, ILogger<MailService> logger)
    {
        _smtpSettings = smtpSettings;
       _configuration = configuration;
       _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string body, string subject)
    {
        if(_configuration.GetValue<bool>("DisableEmails", false))
        {
            _logger.LogInformation("Emails are disabled, not sending email to {Email}.", toEmail);
            return;
        }
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