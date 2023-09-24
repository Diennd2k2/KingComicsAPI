using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingComicsAPI.Models
{
    public class Chapter
    {
        [Key]
        public Guid Chapter_id { get; set; }
        public Guid Comic_id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Arrange { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Comic Comic { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<ReadingHistory> ReadingHistories { get; set; }

    }
}
