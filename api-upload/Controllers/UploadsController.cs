using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using shared.Context;
using shared.Entities;
using shared.Enums;
using System.Runtime.CompilerServices;
using shared.Models;

namespace api_upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private const string QUEUE_NAME = "fila_importacao";

        public UploadsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo inválido.");
            }

            var uploadsFolder = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Shared", "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var newFile = new FileUpload
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                Status = StatusEnum.NEW,
                UploadedDate = DateTime.Now,
            };

            await _appDbContext.Files.AddAsync(newFile);
            await _appDbContext.SaveChangesAsync();

            var fileName = $"{newFile.Id}_{file.FileName}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           // var factory = new ConnectionFactory() { HostName = "localhost" };

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", // <-- nome do serviço no docker-compose
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(new ImportMessage { FileId = newFile.Id, FileName = file.FileName });

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: null,
                 body: body
            );

            return Ok("Arquivo enviado com sucesso.");
        }
    }
}
