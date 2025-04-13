using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using shared.Context;
using shared.Models;
using System.Text.Json;
using shared.Entities;
using Microsoft.EntityFrameworkCore;
using shared.Enums;

namespace console_consumer.Services
{
    public class RabbitConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IModel _channel;
        private IConnection _connection;

        public RabbitConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // var factory = new ConnectionFactory() { HostName = "localhost" };
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", // <-- nome do serviço no docker-compose
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("fila_importacao", false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<FileProcessorService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var data = JsonSerializer.Deserialize<ImportMessage>(message);
                    if (data != null)
                    {
                        await processor.ProcessFileAsync(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                }
            };

            _channel.BasicConsume("fila_importacao", true, consumer);
            Console.WriteLine("Aguardando mensagens...");
            return Task.CompletedTask;
        }


        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
