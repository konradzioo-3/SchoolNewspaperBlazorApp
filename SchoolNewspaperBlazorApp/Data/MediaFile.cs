namespace SchoolNewspaperBlazorApp.Data
{
    public class MediaFile
    {
        public int Id { get; set; }
        public required string FileType { get; set; }
        public required string FileName { get; set; }
        public int? ArticleId { get; set; }
        public bool IsMainImage { get; set; }


    }
}
