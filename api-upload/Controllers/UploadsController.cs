using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using shared.Context;
using shared.Entities;
using shared.Enums;
using System.Runtime.CompilerServices;
using shared.Models;
using Microsoft.Extensions.Options;

namespace api_upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly string _uploadPath;

        private const string QUEUE_NAME = "fila_importacao";

        public UploadsController(AppDbContext appDbContext, IOptions<UploadSettings> uploadSettings)
        {
            _appDbContext = appDbContext;
            _uploadPath = uploadSettings.Value.BasePath;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo inválido.");
            }
           
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }

            var newFile = new FileUpload
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                Status = StatusEnum.NEW,
                UploadedDate = DateTime.UtcNow,
            };

            await _appDbContext.Files.AddAsync(newFile);
            await _appDbContext.SaveChangesAsync();

            var fileName = $"{newFile.Id}_{file.FileName}";

            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

            var factory = new ConnectionFactory()
            {
                HostName = hostName,
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
