using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KingComicsAPI.Context;
using KingComicsAPI.Models;
using KingComicsAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KingComicsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComicController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ComicController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comics = await _context.Comics
                    .Include(c => c.ComicGenres)
                    .ThenInclude(cg => cg.Genre)
                    .ToListAsync();
            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comic = await _context.Comics.Include(c=>c.ComicGenres).ThenInclude(g=>g.Genre).FirstOrDefaultAsync(c => c.Comic_id == id);
            return Ok(comic);
        }

        [HttpGet("genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            try
            {
                var comics = await _context.ComicGenres
                     .Where(cg => cg.Genre.Slug == genre)
                     .Include(cg => cg.Comic)
                     .Include(cg => cg.Genre)
                     .ToListAsync();
                if (comics == null || comics.Count == 0)
                {
                    return NotFound($"Không có truyện tranh nào trong thể loại này.");
                }

                return Ok(comics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"err: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ComicGenreViewModel comicVM)
        {
            if (comicVM.formFile == null || comicVM.formFile.Length == 0)
            {
                return BadRequest("Not Found Image.");
            }
            var account = new Account(
                "dwvtpyyft",
                "187697336685363",
                "IwKBgcSdAvNC1Ilp-1ZKDLM56NU"
            );
            var cloudinary = new Cloudinary(account);

            var now = DateTime.Now;
            var publicId = $"{now:yyyyMMddHHmmss}-{Path.GetFileNameWithoutExtension(comicVM.formFile.FileName)}";
            using(var stream = comicVM.formFile.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(comicVM.formFile.FileName, stream),
                    PublicId = publicId
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                var imageUrl = uploadResult.SecureUrl.ToString();
                var comic = new Comic()
                {
                    Title = comicVM.Title,
                    Slug = comicVM.Slug,
                    CoverImage = imageUrl,
                    Description = comicVM.Description,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ComicGenres = new List<Comic_Genre>()
                };

                foreach (var item in comicVM.GenreIds)
                {
                    comic.ComicGenres.Add(new Comic_Genre()
                    {
                        Comic = comic,
                        Genre_id = item
                    });
                }
                _context.Add(comic);
                await _context.SaveChangesAsync();
            }
                
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ComicGenreViewModel comicVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comic = await _context.Comics.Include(c => c.ComicGenres).ThenInclude(g => g.Genre).FirstOrDefaultAsync(c => c.Comic_id == id);
            if (comic == null)
            {
                return NotFound();
            }

            var account = new Account(
                "dwvtpyyft",
                "187697336685363",
                "IwKBgcSdAvNC1Ilp-1ZKDLM56NU"
            );
            var cloudinary = new Cloudinary(account);

            bool shouldUpdateImage = false;

            if (comicVM.formFile != null && comicVM.formFile.Length != 0)
            {
                shouldUpdateImage = true;

                if (!string.IsNullOrEmpty(comic.CoverImage))
                {
                    var publicIds = comic.CoverImage.Split('/').Last().Split('.').First();
                    var deleteParams = new DeletionParams(publicIds);
                    var deleteResult = await cloudinary.DestroyAsync(deleteParams);
                }

                var now = DateTime.Now;
                var publicId = $"{now:yyyyMMddHHmmss}-{Path.GetFileNameWithoutExtension(comicVM.formFile.FileName)}";
                using (var stream = comicVM.formFile.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(comicVM.formFile.FileName, stream),
                        PublicId = publicId
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    var imageUrl = uploadResult.SecureUrl.ToString();
                    comic.CoverImage = imageUrl;
                }
            }

            if (!shouldUpdateImage)
            {
                comic.CoverImage = comicVM.CoverImage;
            }
            comic.Title = comicVM.Title;
            comic.Slug = comicVM.Slug;
            comic.Description = comicVM.Description;
            comic.Status = comicVM.Status;
            comic.UpdatedAt = DateTime.UtcNow;
            var existingIds = comic.ComicGenres.Select(c => c.Genre_id).ToList();
            var selectedIds = comicVM.GenreIds.ToList();
            var toAdd = selectedIds.Except(existingIds).ToList();
            var toRemove = existingIds.Except(selectedIds).ToList();
            comic.ComicGenres = comic.ComicGenres.Where(x => !toRemove.Contains(x.Genre_id)).ToList();
            foreach (var item in toAdd)
            {
                comic.ComicGenres.Add(new Comic_Genre()
                {
                    Comic_id = comic.Comic_id,
                    Genre_id = item
                });
            }
            _context.Comics.Update(comic);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comic = await _context.Comics.Include(c => c.ComicGenres).FirstOrDefaultAsync(c => c.Comic_id == id);
            var account = new Account(
               "dwvtpyyft",
               "187697336685363",
               "IwKBgcSdAvNC1Ilp-1ZKDLM56NU"
           );

            var cloudinary = new Cloudinary(account);
            if (!string.IsNullOrEmpty(comic.CoverImage))
            {
                var publicIds = comic.CoverImage.Split('/').Last().Split('.').First();
                var deleteParams = new DeletionParams(publicIds);
                var deleteResult = await cloudinary.DestroyAsync(deleteParams);
            }

            _context.ComicGenres.RemoveRange(comic.ComicGenres);
            _context.Comics.Remove(comic);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
