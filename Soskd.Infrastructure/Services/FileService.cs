// Soskd.Infrastructure/Services/FileService.cs - Enhanced with debugging
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Soskd.Infrastructure.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
    }

    public class FileService : IFileService
    {
        private readonly string _webRootPath;
        private readonly ILogger<FileService>? _logger;

        public FileService(string webRootPath, ILogger<FileService>? logger = null)
        {
            _webRootPath = webRootPath;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                _logger?.LogInformation("SaveFileAsync called - File: {FileName}, Folder: {Folder}, WebRootPath: {WebRootPath}",
                    file?.FileName, folder, _webRootPath);

                if (file == null || file.Length == 0)
                {
                    _logger?.LogWarning("File is null or empty");
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(_webRootPath))
                {
                    _logger?.LogError("WebRootPath is null or empty! This is likely the issue.");
                    return string.Empty;
                }

                var uploadsFolder = Path.Combine(_webRootPath, "uploads", folder);
                _logger?.LogInformation("Uploads folder path: {UploadsFolder}", uploadsFolder);

                if (!Directory.Exists(uploadsFolder))
                {
                    _logger?.LogInformation("Creating directory: {UploadsFolder}", uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                _logger?.LogInformation("Full file path: {FilePath}", filePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var relativePath = $"/uploads/{folder}/{uniqueFileName}";
                _logger?.LogInformation("File saved successfully. Relative path: {RelativePath}", relativePath);

                // Verify file was actually saved
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    _logger?.LogInformation("File verification successful. Size: {FileSize} bytes", fileInfo.Length);
                }
                else
                {
                    _logger?.LogError("File was not saved! Path: {FilePath}", filePath);
                    return string.Empty;
                }

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving file: {FileName} to folder: {Folder}", file?.FileName, folder);
                return string.Empty;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                _logger?.LogInformation("DeleteFileAsync called - FilePath: {FilePath}", filePath);

                if (string.IsNullOrEmpty(filePath))
                    return true;

                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/'));
                _logger?.LogInformation("Full delete path: {FullPath}", fullPath);

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger?.LogInformation("File deleted successfully: {FilePath}", filePath);
                }
                else
                {
                    _logger?.LogWarning("File not found for deletion: {FullPath}", fullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/'));
                var exists = File.Exists(fullPath);

                _logger?.LogInformation("FileExists check - Path: {FilePath}, FullPath: {FullPath}, Exists: {Exists}",
                    filePath, fullPath, exists);

                return exists;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking file existence: {FilePath}", filePath);
                return false;
            }
        }
    }
}