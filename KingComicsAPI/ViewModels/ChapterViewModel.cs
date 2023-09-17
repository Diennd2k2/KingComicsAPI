namespace KingComicsAPI.ViewModels
{
    public class ChapterViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Arrange { get; set; }
        public int Views { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
