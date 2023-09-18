using KingComicsAPI.Context;
using KingComicsAPI.Models;
using KingComicsAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ChapterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{comicId}")]
        public async Task<IActionResult> GetChapterByComic(Guid comicId)
        {
            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return NotFound();
            }

            var chapters = await _context.Chapters.Where(c=>c.Comic_id== comicId).OrderByDescending(c=>c.Arrange).ToListAsync();

            return Ok(chapters);
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<IActionResult> GetChapterById(Guid chapterId)
        {
            var chapter = await _context.Chapters.Include(i => i.Images).FirstOrDefaultAsync(c => c.Chapter_id == chapterId);
            chapter.Images = chapter.Images.OrderBy(i => i.Arrange).ToList();

            if (chapter == null)
            {
                return NotFound("Chapter not found.");
            }

            return Ok(chapter);
        }

        [HttpPost("{comicId}")]
        public async Task<IActionResult> Add(Guid comicId, [FromBody] ChapterViewModel chapterVM)
        {
            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null)
            {
                return NotFound("Comic not found");
            }

            var chapter = new Chapter()
            {
                Chapter_id = Guid.NewGuid(),
                Comic_id = comicId,
                Title = chapterVM.Title,
                Slug = chapterVM.Slug,
                Arrange = chapterVM.Arrange,
                Views = chapterVM.Views = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var images = new List<Image>();
            int imageArrange = 1;

            foreach (var imageUrl in chapterVM.ImageUrls)
            {
                var image = new Image
                {
                    Image_id = Guid.NewGuid(),
                    UrlImage = imageUrl,
                    Arrange = imageArrange,
                    Chapter = chapter
                };

                images.Add(image);
                imageArrange++;
            }

            chapter.Images = images;
            comic.UpdatedAt = DateTime.UtcNow;

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{chapterId}")]
        public async Task<IActionResult> Update(Guid chapterId, [FromBody] ChapterViewModel chapterVM)
        {
            var chapter = await _context.Chapters.Include(i => i.Images).FirstOrDefaultAsync(c => c.Chapter_id == chapterId);

            if (chapter == null)
            {
                return NotFound("Chapter not found.");
            }

            _context.Images.RemoveRange(chapter.Images);

            chapter.Title = chapterVM.Title;
            chapter.Slug = chapterVM.Slug;
            chapter.Arrange = chapterVM.Arrange;
            chapter.UpdatedAt = DateTime.UtcNow;

            var newImages = new List<Image>();
            int imageArrange = 1;
            foreach (var imageUrl in chapterVM.ImageUrls)
            {
                var image = new Image
                {
                    Image_id = Guid.NewGuid(),
                    UrlImage = imageUrl,
                    Arrange = imageArrange,
                    Chapter = chapter
                };

                newImages.Add(image);
                imageArrange++;
            }

            _context.Images.AddRange(newImages);

            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{chapterId}")]
        public async Task<IActionResult> Delete(Guid chapterId)
        {
            var chapter = await _context.Chapters.Include(i => i.Images).FirstOrDefaultAsync(c => c.Chapter_id == chapterId);

            if(chapter == null)
            {
                return NotFound("Chapter not found");
            }

            _context.Images.RemoveRange(chapter.Images);
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
