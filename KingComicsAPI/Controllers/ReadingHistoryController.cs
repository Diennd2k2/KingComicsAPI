using KingComicsAPI.Context;
using KingComicsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReadingHistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReadingLog([FromBody] ReadingHistory readingHistory)
        {
            var existingReadingHistory = await _context.ReadingHistories
                .FirstOrDefaultAsync(log =>
                    log.User_id == readingHistory.User_id &&
                    log.Comic_id == readingHistory.Comic_id);

            if (existingReadingHistory == null)
            {
                var newReadingHistory = new ReadingHistory
                {
                    User_id = readingHistory.User_id,
                    Comic_id = readingHistory.Comic_id,
                    Chapter_id = readingHistory.Chapter_id
                };
                _context.ReadingHistories.Add(newReadingHistory);
            }
            else
            {
                _context.ReadingHistories.Remove(existingReadingHistory);
                var newReadingHistory = new ReadingHistory
                {
                    User_id = readingHistory.User_id,
                    Comic_id = readingHistory.Comic_id,
                    Chapter_id = readingHistory.Chapter_id
                };
                _context.ReadingHistories.Add(newReadingHistory);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetComicAndReadingHistory(Guid userId)
        {
            var comicAndHistory = await _context.ReadingHistories
                .Where(log => log.User_id == userId)
                .Include(log => log.Comic)
                .ThenInclude(comic => comic.ComicGenres)
                .ThenInclude(comicGenre => comicGenre.Genre)
                .Include(log => log.Chapter)
                .Select(log => new
                {
                    Comic = new
                    {
                        log.Comic.Comic_id,
                        log.Comic.Title,
                        Genres = log.Comic.ComicGenres.Select(comicGenre => new
                        {
                            comicGenre.Genre.Genre_id,
                            comicGenre.Genre.Genre_Name,
                        }).ToList(),
                    },
                    Chapter = new
                    {
                        log.Chapter.Chapter_id,
                        log.Chapter.Title,
                    },
                    LatestChapter = _context.Chapters
                    .Where(ch => ch.Comic_id == log.Comic.Comic_id)
                    .OrderByDescending(ch => ch.Arrange)
                    .Select(ch => new
                    {
                        ch.Chapter_id,
                        ch.Title,
                    })
                    .FirstOrDefault()
                })
                .ToListAsync();
            comicAndHistory.Reverse();

            return Ok(comicAndHistory);
        }
    }
}
