using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IFileService
    {
        Task<string> GetPreviewAsync(IBrowserFile file);
        Task<int> UploadImage(IBrowserFile file, int? Article = null);
        Task<string> GetFileNameByIdAsync(int id);
        Task<List<string>> GetGalleryFileNamesByIdAsync(int ArticleId);
        // Jak GetFileNameByIdAsync, ale zwraca null zamiast rzucać wyjątek, gdy pliku nie ma.
        Task<string?> TryGetFileNameByIdAsync(int id);
        Task<List<MediaFile>> GetGalleryFilesAsync(int ArticleId);
    }
}
