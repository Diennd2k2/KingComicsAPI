using KingComicsAPI.Models;

namespace KingComicsAPI.ViewModels
{
    public class ComicGenreViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string CoverImage { get; set; }
        public int[] GenreIds { get; set; }

        public IFormFile formFile { get; set; }
    }
}
