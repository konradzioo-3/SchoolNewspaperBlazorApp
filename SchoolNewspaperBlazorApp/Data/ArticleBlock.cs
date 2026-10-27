using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Data
{
    // Fragment treści artykułu. Artykuł składa się z uporządkowanych bloków:
    // tekst, zdjęcie, tekst, zdjęcie... - dzięki temu można pisać w dowolnym miejscu.
    public class ArticleBlock
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
        // Kolejność wyświetlania (0, 1, 2...).
        public int Position { get; set; }
        public EnumArticleBlockType BlockType { get; set; }
        // Dla bloku tekstowego.
        public string? Text { get; set; }
        // Dla bloku ze zdjęciem: Id pliku w tabeli Files.
        public int? MediaFileId { get; set; }
    }

    // Blok w edytorze (jeszcze niezapisany): może trzymać wybrany plik do wgrania.
    public class ArticleBlockDraft
    {
        public Guid Key { get; } = Guid.NewGuid();
        public EnumArticleBlockType Type { get; set; }
        public string? Text { get; set; }
        public IBrowserFile? File { get; set; }
        public int? MediaFileId { get; set; }
        // Adres do pokazania w edytorze i podglądzie (data: dla nowego pliku, /Images/... dla zapisanego).
        public string? PreviewUrl { get; set; }
    }
}
