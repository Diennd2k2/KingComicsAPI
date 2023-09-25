using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KingComicsAPI.Context;
using KingComicsAPI.Models;
using KingComicsAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
                .OrderByDescending(c => c.UpdatedAt).Select(c => new
                {
                    comic_id = c.Comic_id,
                    title = c.Title,
                    slug = c.Slug,
                    coverImage = c.CoverImage,
                    description = c.Description,
                    status = c.Status,
                    views = c.Chapters.Sum(chapter => chapter.Views),
                    createdAt = c.CreatedAt,
                    updatedAt = c.UpdatedAt,
                    comicGenres = c.ComicGenres.Select(cg => new
                    {
                        genre_id = cg.Genre.Genre_id,
                        genre_Name = cg.Genre.Genre_Name,
                    }).ToList(),
                })
                .ToListAsync();

            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comic = await _context.Comics
             .Where(c => c.Comic_id == id)
             .Select(c => new
             {
                 comic_id = c.Comic_id,
                 title = c.Title,
                 coverImage = c.CoverImage,
                 description = c.Description,
                 status = c.Status,
                 views = c.Chapters.Sum(chapter => chapter.Views),
                 genre = c.ComicGenres.Select(g => new
                 {
                     genre_id = g.Genre.Genre_id,
                     genre_Name = g.Genre.Genre_Name,
                 }).ToList()
             })
             .FirstOrDefaultAsync();
            return Ok(comic);
        }

        [HttpGet("genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
        {
            try
            {
                var comics = await _context.ComicGenres
                     .Where(cg => cg.Genre.Slug == genre)
                     .Select(cg => new
                     {
                         comic_id = cg.Comic.Comic_id,
                         title = cg.Comic.Title,
                         slug = cg.Comic.Slug,
                         coverImage = cg.Comic.CoverImage,
                         description = cg.Comic.Description,
                         status = cg.Comic.Status,
                         views = cg.Comic.Chapters.Sum(chapter => chapter.Views),
                         createdAt = cg.Comic.CreatedAt,
                         updatedAt = cg.Comic.UpdatedAt,
                         comicGenres = cg.Comic.ComicGenres
                        .Select(g => new
                        {
                            genre_id = g.Genre.Genre_id,
                            genre_Name = g.Genre.Genre_Name,
                        })
                        .ToList()
                     })
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

        [Authorize]
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
            using (var stream = comicVM.formFile.OpenReadStream())
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

        [Authorize]
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

        [Authorize]
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

        [HttpGet("total-comics")]
        public async Task<ActionResult<int>> GetTotalNumberOfComics()
        {
            try
            {
                int totalComics = await _context.Comics.CountAsync();

                return Ok(totalComics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("views/{comicId}")]
        public async Task<ActionResult<int>> GetTotalComicViews(Guid comicId)
        {
            try
            {
                int totalViews = await _context.Chapters
                    .Where(chapter => chapter.Comic_id == comicId)
                    .SumAsync(chapter => chapter.Views);

                return Ok(totalViews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("total-chapters/{comicId}")]
        public async Task<ActionResult<int>> GetTotalChaptersOfComic(Guid comicId)
        {
            try
            {
                int totalChapters = await _context.Chapters
                    .Where(chapter => chapter.Comic_id == comicId)
                    .CountAsync();

                return Ok(totalChapters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-5-comics")]
        public async Task<ActionResult<IEnumerable<Comic>>> GetTop5Comics()
        {
            try
            {
                var top5Comics = await _context.Comics
                    .OrderByDescending(comic => comic.Chapters.Sum(chapter => chapter.Views))
                    .ThenByDescending(comic => comic.UpdatedAt)
                    .Take(10).Select(c => new
                    {
                        comic_id = c.Comic_id,
                        title = c.Title,
                        slug = c.Slug,
                        coverImage = c.CoverImage,
                        description = c.Description,
                        status = c.Status,
                        views = c.Chapters.Sum(chapter => chapter.Views),
                        createdAt = c.CreatedAt,
                        updatedAt = c.UpdatedAt,
                        comicGenres = c.ComicGenres.Select(cg => new
                        {
                            genre_id = cg.Genre.Genre_id,
                            genre_Name = cg.Genre.Genre_Name,
                        }).ToList(),
                    })

                    .ToListAsync();

                return Ok(top5Comics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
