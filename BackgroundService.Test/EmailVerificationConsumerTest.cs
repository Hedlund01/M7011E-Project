using BackgroundService.Console.Consumers;
using BackgroundService.Console.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLib.Models.DTOs;

namespace BackgroundService.Test;

public class EmailVerificationConsumerTest
{
    [Fact]
    public async Task Consume_TwoFactorEmail_ShouldSendEmail()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailVerificationConsumer>>();
        var mailService = new Mock<IMailService>();
        var context = new Mock<ConsumeContext<TwoFactorEmail>>();
        var email = "test@ltu.lol";
        var body = "Hi, your 2fa token is: \n";
        var subject = "2fa token - M7011E";
        var token = "123456";

        context.Setup(l => l.Message).Returns(new TwoFactorEmail { Token = token, Email = email });
        
        mailService.Setup(lib => lib.SendEmailAsync(email, body, subject)).Verifiable();
        

        var consumer = new EmailVerificationConsumer(logger.Object, mailService.Object);
        await consumer.Consume(context.Object);
        mailService.Verify(l => l.SendEmailAsync(email, body+token, subject), Times.Once);
        
    }

    [Fact]
    public async Task Consume_EmailVerification_ShouldSendEmail()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailVerificationConsumer>>();
        var mailService = new Mock<IMailService>();
        var context = new Mock<ConsumeContext<EmailVerification>>();
        var email = "test@ltu.lol";
        var body = "Hi, your email verification token is: \n";
        var subject = "email verification token - M7011E";
        var token = "123456";
        
        context.Setup(l => l.Message).Returns(new EmailVerification { Token = token, Email = email });
        
        mailService.Setup(lib => lib.SendEmailAsync(email, body, subject)).Verifiable();
        

        var consumer = new EmailVerificationConsumer(logger.Object, mailService.Object);
        await consumer.Consume(context.Object);
        mailService.Verify(l => l.SendEmailAsync(email, body+token, subject), Times.Once);
    }


}