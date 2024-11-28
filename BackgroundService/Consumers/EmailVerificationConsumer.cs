using BackgroundService.Console.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib.Models.DTOs;

namespace BackgroundService.Console.Consumers;

public class EmailVerificationConsumer(ILogger<EmailVerificationConsumer> logger, IMailService mailService)
    : IConsumer<TwoFactorEmail>, IConsumer<EmailVerification>
{

    public async Task Consume(ConsumeContext<TwoFactorEmail> context)
    {
       await mailService.SendEmailAsync(context.Message.Email, $"Hi, your 2fa token is: \n{context.Message.Token}",
            "2fa token - M7011E");
        logger.LogInformation("2fa email verification sent to {Email}, token: {Token}", context.Message.Email, context.Message.Token);
    }

    public async  Task Consume(ConsumeContext<EmailVerification> context)
    {
        await mailService.SendEmailAsync(context.Message.Email, $"Hi, your email verification token is: \n{context.Message.Token}",
            "email verification token - M7011E");
        logger.LogInformation("Email verification sent to {Email}, token: {Token}", context.Message.Email, context.Message.Token);
    }
    
    
    
}