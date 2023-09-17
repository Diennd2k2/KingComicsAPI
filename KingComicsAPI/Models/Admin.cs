using System.ComponentModel.DataAnnotations;

namespace KingComicsAPI.Models
{
    public class Admin
    {
        [Key]
        public Guid Admin_id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
