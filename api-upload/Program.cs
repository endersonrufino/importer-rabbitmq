using Microsoft.EntityFrameworkCore;
using shared.Context;
using shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Verifica se está rodando no Docker via variável de ambiente
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (isDocker)
{
    // No Docker, escuta na porta 8080 (ou qualquer outra que desejar)
    builder.WebHost.UseUrls("http://+:80");
}

builder.Services.Configure<UploadSettings>(
    builder.Configuration.GetSection("UploadSettings"));

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("UsersDb");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
