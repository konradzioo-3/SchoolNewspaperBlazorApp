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
        public string? AuthorRole { get; set; }
        public DateTime PublishDate { get; set; }
        public int MainImageId { get; set; }
        public EnumArticleType ArticleType { get; set; }
        // Soft delete: false = 0 (aktywny), true = 1 (usunięty). Usunięte rekordy zostają w bazie.
        public bool IsDeleted { get; set; }

    }

    
}
