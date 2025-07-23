// Soskd.Api/Controllers/FileController.cs
using Microsoft.AspNetCore.Mvc;
using Soskd.Infrastructure.Services;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Upload a file to the specified folder
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="folder">The folder to save the file in (e.g., "news", "media")</param>
        /// <returns>The URL of the uploaded file</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string folder = "general")
        {
            try
            {
                _logger.LogInformation("AAA - API File upload request: {FileName}, Folder: {Folder}", file?.FileName, folder);

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided or file is empty." });
                }

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File size cannot exceed 10MB." });
                }

                // AAA - Validate file type based on folder
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                bool isValidFile = false;
                string allowedTypesMessage = "";

                switch (folder.ToLowerInvariant())
                {
                    case "news":
                    case "media":
                        // Image files for news and media
                        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        isValidFile = imageExtensions.Contains(fileExtension);
                        allowedTypesMessage = "Only image files (jpg, jpeg, png, gif, webp) are allowed for this folder.";
                        break;

                    case "documents":
                        // PDF files for documents
                        var documentExtensions = new[] { ".pdf" };
                        isValidFile = documentExtensions.Contains(fileExtension);
                        allowedTypesMessage = "Only PDF files are allowed for documents folder.";
                        break;

                    default:
                        // General files - allow both images and PDFs
                        var generalExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
                        isValidFile = generalExtensions.Contains(fileExtension);
                        allowedTypesMessage = "Only image files (jpg, jpeg, png, gif, webp) and PDF files are allowed.";
                        break;
                }

                if (!isValidFile)
                {
                    return BadRequest(new { message = allowedTypesMessage });
                }

                var fileUrl = await _fileService.SaveFileAsync(file, folder);

                if (string.IsNullOrEmpty(fileUrl))
                {
                    return StatusCode(500, new { message = "Failed to save file." });
                }

                _logger.LogInformation("AAA - File uploaded successfully: {FileUrl}", fileUrl);

                return Ok(new
                {
                    success = true,
                    fileUrl = fileUrl,
                    fileName = file.FileName,
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error uploading file");
                return StatusCode(500, new { message = "An error occurred while uploading the file.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="filePath">The relative path of the file to delete (e.g., "/uploads/news/filename.jpg")</param>
        /// <returns>Success status</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string filePath)
        {
            try
            {
                _logger.LogInformation("AAA - API File delete request: {FilePath}", filePath);

                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "File path is required." });
                }

                var success = await _fileService.DeleteFileAsync(filePath);

                _logger.LogInformation("AAA - File delete result: {Success}", success);

                return Ok(new { success = success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error deleting file: {FilePath}", filePath);
                return StatusCode(500, new { message = "An error occurred while deleting the file.", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="filePath">The relative path of the file to check</param>
        /// <returns>File existence status</returns>
        [HttpGet("exists")]
        public IActionResult FileExists([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "File path is required." });
                }

                var exists = _fileService.FileExists(filePath);

                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error checking file existence: {FilePath}", filePath);
                return StatusCode(500, new { message = "An error occurred while checking file existence.", error = ex.Message });
            }
        }
    }
}