using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shared.Context;

namespace api_upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _context.Files.ToListAsync();

            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                return NotFound("Arquivo nãop encontrado!");
            }

            return Ok(file);
        }
    }
}
