using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BackgroundService.Console.Consumers;
using BackgroundService.Console.Models;
using BackgroundService.Console.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


var isService = !(Debugger.IsAttached || args.Contains("--console"));

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();

        if (args != null)
            config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailVerificationConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("amqps://fpdfbymx:xxIaESqeUtzHkNrsiV6zT6licszNR52x@hog.rmq5.cloudamqp.com/fpdfbymx");
                cfg.ConfigureEndpoints(ctx);
            });
        });

        var smtpSettings = hostContext.Configuration.GetSection("SMTP").Get<SmtpSettings>();
        services.AddSingleton(smtpSettings);
        services.AddScoped<MailService>();

    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

if (isService)
{
    await builder.Build().RunAsync();
}
else
{
    await builder.RunConsoleAsync();
}