using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using shared.Context;
using console_consumer.Services;
using shared.Models;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true)
              .AddEnvironmentVariables(); // Permite usar UploadSettings__BasePath no docker-compose
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Configurações e DI
        services.Configure<UploadSettings>(configuration.GetSection("UploadSettings"));
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("UsersDb"));

        services.AddScoped<FileProcessorService>();
        services.AddHostedService<RabbitConsumerService>();
    })
    .Build();

await host.RunAsync();
