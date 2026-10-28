using System.Collections.Generic;

namespace SchoolNewspaperBlazorApp.Data.Enums
{
    public enum EnumPermissions
    {
        AddArticle,
        EditArticle,
        DeleteArticle,
        AddUser,
        ManageUser,
        AddFile,
        Master,

        AddSubstituteLesson,
        // Nowe uprawnienia dodajemy zawsze na końcu - wartości liczbowe zapisane w bazie nie mogą się zmienić.
        ManageClasses,
        ManageTeachers,
        ManageRooms,
        ManageSubjects,
        ManageCustomFields
    }
}