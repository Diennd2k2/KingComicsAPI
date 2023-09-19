using Humanizer.Localisation;
using KingComicsAPI.Context;
using KingComicsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GenreController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _context.Genres.OrderBy(g=>g.Genre_id).ToListAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g=>g.Genre_id==id);
            return Ok(genre);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Genre genre)
        {
            genre.status = 1;
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return Ok(genre);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Genre genre)
        {
            var gen = await _context.Genres.FirstOrDefaultAsync(g=>g.Genre_id==id);
            gen.Genre_Name = genre.Genre_Name;
            gen.Slug = genre.Slug;
            gen.status = genre.status;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var gen = await _context.Genres.FirstOrDefaultAsync(g => g.Genre_id == id);
            _context.Genres.Remove(gen);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
