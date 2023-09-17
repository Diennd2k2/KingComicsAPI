using System.ComponentModel.DataAnnotations.Schema;

namespace KingComicsAPI.Models
{
    public class Comic_Genre
    {
        public Guid Comic_id { get; set; }
        public int Genre_id { get; set; }

        public Comic Comic { get; set; }
        public Genre Genre { get; set; }
    }
}
