using SchoolNewspaperBlazorApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolNewspaperBlazorApp.Data
{
    public class Article
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Text { get; set; }
        public required string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public int MainImageId { get; set; }
        public EnumArticleType ArticleType { get; set; }
        public SubstituteLessons? SubstituteDetails { get; set; }

    }

    
}
