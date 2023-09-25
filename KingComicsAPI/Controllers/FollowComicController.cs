using KingComicsAPI.Context;
using KingComicsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowComicController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FollowComicController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> FollowComic([FromBody] FollowComic followComic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FollowComics.Add(followComic);
            await _context.SaveChangesAsync();
            /*return CreatedAtAction("GetFollowComic", new { id = followComic.User_id }, followComic);*/
            return Ok();
        }

        [HttpDelete("{userId}/{comicId}")]
        public async Task<IActionResult> UnfollowComic([FromRoute] Guid userId, Guid comicId)
        {
            var followComic = await _context.FollowComics.SingleOrDefaultAsync(fc => fc.User_id == userId && fc.Comic_id == comicId);

            if (followComic == null)
            {
                return NotFound();
            }

            _context.FollowComics.Remove(followComic);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFollowedComics([FromRoute] Guid userId)
        {
            var followedComics = await _context.FollowComics
                .Where(fc => fc.User_id == userId)
                .Select(c => new
                {
                    comic_id = c.Comic.Comic_id,
                    title = c.Comic.Title,
                    coverImage = c.Comic.CoverImage,
                    views = c.Comic.Chapters.Sum(chapter => chapter.Views),
                    comicGenres = c.Comic.ComicGenres
                    .Select(g => new
                    {
                        genre_id = g.Genre.Genre_id,
                        genre_Name = g.Genre.Genre_Name,
                    }).ToList(),
                    readingHistory = _context.ReadingHistories.Where(log => log.User_id == userId && log.Comic_id == c.Comic.Comic_id)
                    .Select(log => new
                    {
                        chapter_id = log.Chapter.Chapter_id,
                        title = log.Chapter.Title,
                    }).ToList(),
                })
                .ToListAsync();
            followedComics.Reverse();
            return Ok(followedComics);
        }

        [HttpGet("check/{userId}/{comicId}")]
        public IActionResult CheckFollowStatus(Guid userId, Guid comicId)
        {
            bool isFollowed = _context.FollowComics
                .Any(fc => fc.User_id == userId && fc.Comic_id == comicId);

            return Ok(new { isFollowed });
        }
    }
}
