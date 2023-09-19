using Humanizer.Localisation;
using KingComicsAPI.Context;
using KingComicsAPI.Helpers;
using KingComicsAPI.Models;
using KingComicsAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;

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

        [HttpPost("register")]
        public async Task<IActionResult> Add([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            if (await CheckEmailExistAsync(userObj.Email))
            {
                return BadRequest(new { Message = "Email Already Exist!" });
            }

            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
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

        /*[HttpPut("{id}")]
        public async Task<IActionResult> changePassword(Guid id, [FromBody] User user)
        {
            var u = await _context.Users.FirstOrDefaultAsync(g => g.User_id == id);
            u.Password = PasswordHasher.HashPassword(user.Password);
            await _context.SaveChangesAsync();
            return Ok();
        }*/

        [HttpPut("{userId}")]
        public async Task<IActionResult> updateUser(Guid userId, [FromBody] User userObj)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.User_id == userId);
            if (user == null)
                return BadRequest();
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
            }

            user.NickName = userObj.NickName;
            user.Email = userObj.Email;
            user.Password = PasswordHasher.HashPassword(userObj.Password);
            user.Avatar = userObj.Avatar;
            user.Role = userObj.Role;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "User update success!" });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.User_id == userId);
            return Ok(user);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAdmin(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.User_id == userId);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private Task<bool> CheckEmailExistAsync(string email) => _context.Admins.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
            {
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
            {
                sb.Append("Password should contain special chars" + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
