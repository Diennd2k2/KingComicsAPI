using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingComicsAPI.Models
{
    public class Image
    {
        [Key]
        public Guid Image_id { get; set; }
        public string UrlImage { get; set; }
        public int Arrange { get; set; }
        public Guid Chapter_id { get; set; }

        public Chapter Chapter { get; set; }
    }
}
