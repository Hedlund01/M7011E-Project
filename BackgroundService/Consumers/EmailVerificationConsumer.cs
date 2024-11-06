using MassTransit;
using SharedLib.Models.DTOs;

namespace BackgroundService.Consumers;

public class EmailVerificationConsumer(ILogger<EmailVerificationConsumer> logger)
    : IConsumer<SendEmailEmailVerification>
{

    public Task Consume(ConsumeContext<SendEmailEmailVerification> context)
    {
        logger.LogInformation($"Email verification sent to {context.Message.Email}, token: {context.Message.Token}");
        return Task.CompletedTask;
    }
}