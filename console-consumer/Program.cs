using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using shared.Context;
using console_consumer.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("UsersDb"));

        services.AddScoped<FileProcessorService>();
        services.AddHostedService<RabbitConsumerService>();
    })
    .Build();

await host.RunAsync();
