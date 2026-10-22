using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly string _uploadPath;
        public FileService(IFileRepository fileRepository, IWebHostEnvironment environment,IPermissionService permissionService,INotificationService notificationService)
        {
            _fileRepository = fileRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _uploadPath = Path.Combine(environment.ContentRootPath, "Images");
        }
        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }
        public async Task<int> UploadImage(IBrowserFile file,int? ArticleId = null)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddArticle);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnien!",EnumNotificationType.Failure);
                throw new Exception("Brak uprawnien!");
            }
            //Tworzenie folderu
            Directory.CreateDirectory(_uploadPath);
            //Pobieranie extension z pliku czyli np .png
            string extension = Path.GetExtension(file.Name);
            //Generowanie nazwy + extension np. plik.png
            string fileName = $"{Guid.NewGuid()}{extension}";
            // łaczenie uploadPath i fileName C:\Users\Praktyka\source\repos\SchoolNewspaperBlazorApp\Images\plik.png
            string fullPath = Path.Combine(_uploadPath, fileName);

            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var fileStream = new FileStream(
                fullPath, //pelny link odnosnika
                FileMode.Create //ustawienie tworzenia
                );
            await stream.CopyToAsync(fileStream);
            var id = await _fileRepository.GetLastFileId();
            var mediaFile = new MediaFile
            {
                Id = id,
                FileName = fileName,
                FileType = extension,
                ArticleId = ArticleId,
                IsMainImage = ArticleId == null 
            };
            await _fileRepository.AddFileAsync(mediaFile);

            return id;

        }
        public async Task<string> GetFileNameByIdAsync(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);
            return file.FileName;
        }
        public async Task<List<string>> GetGalleryFileNamesByIdAsync(int ArticleId)
        {
            var files = await _fileRepository.GetGalleryFileNamesByIdAsync(ArticleId);
            return files.Select(file => file.FileName).ToList();
        }
    }
}
