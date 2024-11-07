using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLib.Models.DTOs;

namespace BackgroundService.Console.Consumers;

public class EmailVerificationConsumer(ILogger<EmailVerificationConsumer> logger)
    : IConsumer<TwoFactorEmail>, IConsumer<EmailVerification>
{

    public Task Consume(ConsumeContext<TwoFactorEmail> context)
    {
        logger.LogInformation("2fa email verification sent to {Email}, token: {Token}", context.Message.Email, context.Message.Token);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<EmailVerification> context)
    {
        logger.LogInformation("Email verification sent to {Email}, token: {Token}", context.Message.Email, context.Message.Token);
        return Task.CompletedTask;
    }
}