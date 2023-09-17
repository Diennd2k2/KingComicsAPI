using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KingComicsAPI.Models
{
    public class Genre
    {
        [Key]
        public int Genre_id { get; set; }
        public string Genre_Name { get; set; }
        public string Slug { get; set; }
        public int status { get; set; }

        public ICollection<Comic_Genre> ComicGenres { get; set; }

    }
}
