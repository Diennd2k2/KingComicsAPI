namespace KingComicsAPI.Models
{
    public class ReadingHistory
    {
        public Guid User_id { get; set; }
        public Guid Comic_id { get; set; }
        public Guid Chapter_id { get; set; }

        public User User { get; set; }
        public Comic Comic { get; set; }
        public Chapter Chapter { get; set; }
    }
}
