using System.ComponentModel.DataAnnotations;

namespace KingComicsAPI.Models
{
    public class User
    {
        [Key]
        public Guid User_id { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<FollowComic> FollowComics { get; set;}
        public ICollection<ReadingHistory> ReadingHistories { get; set;}
    }
}
