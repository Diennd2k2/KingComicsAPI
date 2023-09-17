using KingComicsAPI.Context;
using KingComicsAPI.Helpers;
using KingComicsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;   
        } 

        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin([FromBody] Admin adminObj)
        {
            if (adminObj == null)
                return BadRequest();
            var admin = await _context.Admins.FirstOrDefaultAsync(x=>x.Email == adminObj.Email);
            if (admin == null)
                return NotFound(new { Message = "Admin Not Found!" });

            if (!PasswordHasher.VerifyPassword(adminObj.Password, admin.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            var token = CreateJwt(admin);

            return Ok(new { Token = token, Message = "Login Success" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] Admin adminObj)
        {
            if(adminObj == null)
                return BadRequest();
            if (await CheckEmailExistAsync(adminObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            var pass = CheckPasswordStrength(adminObj.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
            }

            adminObj.Admin_id = Guid.NewGuid();
            adminObj.Password = PasswordHasher.HashPassword(adminObj.Password);
            adminObj.Role = "admin";
            adminObj.CreatedAt = DateTime.UtcNow;
            adminObj.UpdatedAt = DateTime.UtcNow;
            await _context.Admins.AddAsync(adminObj);
            await _context.SaveChangesAsync();
            return Ok(new {Message="Admin register success!"});
        }

        private Task<bool> CheckEmailExistAsync(string email)=> _context.Admins.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
            {
                sb.Append("Minimum password length should be 8"+Environment.NewLine);
            }
            if(!(Regex.IsMatch(password,"[a-z]") && Regex.IsMatch(password,"[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should be Alphanumeric"+Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
            {
                sb.Append("Password should contain special chars" + Environment.NewLine);
            }
            return sb.ToString();
        }

        private string CreateJwt(Admin admin)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("kingcomics......");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim(ClaimTypes.Name,$"{admin.FullName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet]        
        public async Task<ActionResult<Admin>> GetAllAdmin()
        {
            return Ok(await _context.Admins.ToListAsync());
        }
    }
}
