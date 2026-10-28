namespace SchoolNewspaperBlazorApp.Data
{
    // Wspólna baza dla słowników szkolnych (klasy, sale, przedmioty).
    // Sama nie jest encją - jej właściwości trafiają do tabel klas pochodnych.
    public abstract class DictionaryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Soft delete: false = 0 (aktywny), true = 1 (usunięty). Usunięte rekordy zostają w bazie.
        public bool IsDeleted { get; set; }
    }

    public class SchoolClass : DictionaryItem { }

    public class Room : DictionaryItem { }

    public class Subject : DictionaryItem { }
}
