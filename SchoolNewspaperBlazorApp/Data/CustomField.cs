using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Data
{
    // Własne pole formularza zastępstw, zdefiniowane przez administratora.
    public class CustomField
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public EnumCustomFieldType FieldType { get; set; }
        // Dla typu Select: opcje oddzielone średnikiem.
        public string? Options { get; set; }
        public bool IsRequired { get; set; }
        // Czy kolumna z tym polem ma być widoczna na publicznej liście zastępstw.
        public bool ShowOnList { get; set; } = true;
        public int SortOrder { get; set; }
        // Soft delete: wartości wpisane wcześniej zostają w bazie.
        public bool IsDeleted { get; set; }

        public string[] GetOptions() =>
            (Options ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public class SubstituteFieldValue
    {
        public int Id { get; set; }
        public int SubstituteLessonId { get; set; }
        public SubstituteLessons SubstituteLesson { get; set; } = null!;
        public int CustomFieldId { get; set; }
        public CustomField CustomField { get; set; } = null!;
        public string Value { get; set; } = string.Empty;
    }
}
