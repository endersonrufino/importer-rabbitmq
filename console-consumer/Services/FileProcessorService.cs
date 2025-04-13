using console_consumer.Helpers;
using Microsoft.EntityFrameworkCore;
using shared.Context;
using shared.Enums;
using shared.Models;

namespace console_consumer.Services
{
    public class FileProcessorService
    {
        private readonly AppDbContext _dbContext;

        public FileProcessorService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ProcessFileAsync(ImportMessage data)
        {
            var uploadsFolder = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "Shared", "Uploads");

            var fileName = $"{data.FileId}_{data.FileName}";

            var filePath = Path.Combine(uploadsFolder, fileName);

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
