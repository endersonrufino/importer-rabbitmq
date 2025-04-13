using Microsoft.EntityFrameworkCore;
using shared.Context;

var builder = WebApplication.CreateBuilder(args);

// Forçar escuta na porta 80 quando rodando no Docker
//builder.WebHost.UseUrls("http://+:80");

// Add services
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("UsersDb");
});

var app = builder.Build();

// Middlewares
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
