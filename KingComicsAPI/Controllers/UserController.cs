using Humanizer.Localisation;
using KingComicsAPI.Context;
using KingComicsAPI.Helpers;
using KingComicsAPI.Models;
using KingComicsAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userObj.Email);
            if (user == null)
                return NotFound(new { Message = "User Not Found!" });

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }


            return Ok(new { Message = "Login Success" });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }
                
            userObj.User_id = Guid.NewGuid();
            userObj.Avatar = "";
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "1";
            userObj.CreatedAt = DateTime.UtcNow;
            userObj.UpdatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(userObj);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "User register success!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> changePassword(Guid id, [FromBody] User user)
        {
            var u = await _context.Users.FirstOrDefaultAsync(g => g.User_id == id);
            u.Password = PasswordHasher.HashPassword(user.Password);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
