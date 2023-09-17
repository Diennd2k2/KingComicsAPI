using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KingComicsAPI.Models
{
    public class Comic
    {
        [Key]
        public Guid Comic_id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<FollowComic> FollowComics { get; set; }
        public ICollection<Comic_Genre> ComicGenres { get; set; }
    }
}
