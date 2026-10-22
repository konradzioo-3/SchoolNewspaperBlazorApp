using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public FileRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<int> GetLastFileId()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var id = await _context.Files
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            return id + 1;
        }
        public async Task<MediaFile> GetFileByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var fileId = await _context.Files.FirstOrDefaultAsync(a => a.Id == id);
            if(fileId == null)
            {
                throw new Exception("[FileRepository]Brak zdjecia w bazie danych");
            }
            return fileId;
        }
        public async Task AddFileAsync(MediaFile file)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();
        }
        public async Task<List<MediaFile>> GetGalleryFileNamesByIdAsync(int ArticleId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Files.Where(f => f.ArticleId == ArticleId).ToListAsync();
        }

    }
}
