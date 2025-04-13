using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shared.Context;

namespace api_upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
      
      
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }
    }
}
