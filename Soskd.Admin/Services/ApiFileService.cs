// Soskd.Admin/Services/ApiFileService.cs
using System.Text.Json;

namespace Soskd.Admin.Services
{
    public interface IApiFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
        string GetApiFileUrl(string relativePath);
    }

    public class ApiFileService : IApiFileService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiFileService> _logger;
        private readonly string _apiBaseUrl;

        public ApiFileService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiFileService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // AAA - Get API base URL from configuration or use default
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://127.0.0.1:5000";
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                _logger.LogInformation("AAA - Uploading file to API: {FileName}, Folder: {Folder}", file.FileName, folder);

                if (file == null || file.Length == 0)
                    return string.Empty;

                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, "file", file.FileName);
                content.Add(new StringContent(folder), "folder");

                var response = await _httpClient.PostAsync("/api/file/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiFileUploadResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("AAA - File uploaded successfully to API: {FileUrl}", result?.FileUrl);
                    return result?.FileUrl ?? string.Empty;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("AAA - Failed to upload file to API. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error uploading file to API");
                return string.Empty;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("AAA - Deleting file from API: {FilePath}", filePath);

                if (string.IsNullOrEmpty(filePath))
                    return true;

                var response = await _httpClient.DeleteAsync($"/api/file/delete?filePath={Uri.EscapeDataString(filePath)}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiFileDeleteResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("AAA - File delete result from API: {Success}", result?.Success);
                    return result?.Success ?? false;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("AAA - Failed to delete file from API. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error deleting file from API");
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var response = _httpClient.GetAsync($"/api/file/exists?filePath={Uri.EscapeDataString(filePath)}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var result = JsonSerializer.Deserialize<ApiFileExistsResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result?.Exists ?? false;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error checking file existence in API");
                return false;
            }
        }

        /// <summary>
        /// AAA - Convert relative file path to full API URL for displaying images
        /// </summary>
        public string GetApiFileUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            // Remove leading slash if present
            var cleanPath = relativePath.TrimStart('/');
            return $"{_apiBaseUrl}/{cleanPath}";
        }
    }

    // AAA - Response models for API calls
    public class ApiFileUploadResponse
    {
        public bool Success { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    public class ApiFileDeleteResponse
    {
        public bool Success { get; set; }
    }

    public class ApiFileExistsResponse
    {
        public bool Exists { get; set; }
    }
}