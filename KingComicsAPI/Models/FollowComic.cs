using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingComicsAPI.Models
{
    public class FollowComic
    {
        public Guid User_id { get; set; }
        public Guid Comic_id { get; set; }

        public User User { get; set; }
        public Comic Comic { get; set; }
    }
}
