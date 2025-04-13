using console_consumer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using shared.Context;
using shared.Enums;
using shared.Models;

namespace console_consumer.Services
{
    public class FileProcessorService
    {
        private readonly AppDbContext _dbContext;
        private readonly string _uploadPath;

        public FileProcessorService(AppDbContext dbContext, IOptions<UploadSettings> options)
        {
            _dbContext = dbContext;
            _uploadPath = options.Value.BasePath;
        }

        public async Task ProcessFileAsync(ImportMessage data)
        {           
            var fileName = $"{data.FileId}_{data.FileName}";

            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo {filePath} não encontrado.");
                return;
            }

            var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == data.FileId);
            if (file != null)
            {
                file.Status = StatusEnum.PENDING;
                await _dbContext.SaveChangesAsync();
            }

            var lines = await File.ReadAllLinesAsync(filePath);

            var users = CsvParser.ParseUsers(lines);

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            if (file != null)
            {
                file.Status = StatusEnum.COMPLETED;
                await _dbContext.SaveChangesAsync();
            }

            Console.WriteLine($"Arquivo {data.FileName} processado com sucesso.");
        }
    }
}
