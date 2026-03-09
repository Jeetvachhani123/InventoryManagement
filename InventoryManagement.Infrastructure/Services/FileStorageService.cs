using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var folder = Path.Combine("wwwroot", "images");
        Directory.CreateDirectory(folder);

        var uniqueName = Guid.NewGuid() + Path.GetExtension(fileName);
        var path = Path.Combine(folder, uniqueName);

        using var stream = new FileStream(path, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return $"/images/{uniqueName}";
    }
}
