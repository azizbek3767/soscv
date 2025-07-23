// Soskd.Api/Controllers/DocumentsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all published documents with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="category">Filter by category (optional)</param>
        /// <param name="search">Search in titles and descriptions (optional)</param>
        /// <returns>Paginated list of published documents</returns>
        [HttpGet]
        public async Task<ActionResult<DocumentListDto>> GetDocuments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null)
        {
            try
            {
                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var query = _context.Documents
                    .Where(d => d.Status == DocumentStatus.Published)
                    .AsQueryable();

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(d => d.CategoryEn == category ||
                                           d.CategoryUz == category ||
                                           d.CategoryRu == category);
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(d => d.TitleEn.ToLower().Contains(searchLower) ||
                                           d.TitleUz.ToLower().Contains(searchLower) ||
                                           d.TitleRu.ToLower().Contains(searchLower) ||
                                           (d.DescriptionEn != null && d.DescriptionEn.ToLower().Contains(searchLower)) ||
                                           (d.DescriptionUz != null && d.DescriptionUz.ToLower().Contains(searchLower)) ||
                                           (d.DescriptionRu != null && d.DescriptionRu.ToLower().Contains(searchLower)));
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var documents = await query
                    .OrderByDescending(d => d.PublishDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DocumentItemDto
                    {
                        Id = d.Id,
                        TitleUz = d.TitleUz,
                        TitleRu = d.TitleRu,
                        TitleEn = d.TitleEn,
                        DescriptionUz = TruncateText(d.DescriptionUz, 200),
                        DescriptionRu = TruncateText(d.DescriptionRu, 200),
                        DescriptionEn = TruncateText(d.DescriptionEn, 200),
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added multilingual file properties
                        FileUrl = d.FileUrl, // Russian (keeping original)
                        FileName = d.FileName,
                        FileSizeBytes = d.FileSizeBytes,
                        FileSizeFormatted = d.FileSizeFormatted,
                        FileUrlUz = d.FileUrlUz, // Uzbek
                        FileNameUz = d.FileNameUz,
                        FileSizeBytesUz = d.FileSizeBytesUz,
                        FileSizeFormattedUz = d.FileSizeFormattedUz,
                        FileUrlEn = d.FileUrlEn, // English
                        FileNameEn = d.FileNameEn,
                        FileSizeBytesEn = d.FileSizeBytesEn,
                        FileSizeFormattedEn = d.FileSizeFormattedEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of multilingual file properties
                        CategoryUz = d.CategoryUz,
                        CategoryRu = d.CategoryRu,
                        CategoryEn = d.CategoryEn,
                        PublishDate = d.PublishDate
                    })
                    .ToListAsync();

                var result = new DocumentListDto
                {
                    Documents = documents,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching documents.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific document by ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Document details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> GetDocument(int id)
        {
            try
            {
                var document = await _context.Documents
                    .Where(d => d.Id == id && d.Status == DocumentStatus.Published)
                    .Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        TitleUz = d.TitleUz,
                        TitleRu = d.TitleRu,
                        TitleEn = d.TitleEn,
                        DescriptionUz = d.DescriptionUz,
                        DescriptionRu = d.DescriptionRu,
                        DescriptionEn = d.DescriptionEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added multilingual file properties
                        FileUrl = d.FileUrl, // Russian (keeping original)
                        FileName = d.FileName,
                        FileSizeBytes = d.FileSizeBytes,
                        FileSizeFormatted = d.FileSizeFormatted,
                        FileUrlUz = d.FileUrlUz, // Uzbek
                        FileNameUz = d.FileNameUz,
                        FileSizeBytesUz = d.FileSizeBytesUz,
                        FileSizeFormattedUz = d.FileSizeFormattedUz,
                        FileUrlEn = d.FileUrlEn, // English
                        FileNameEn = d.FileNameEn,
                        FileSizeBytesEn = d.FileSizeBytesEn,
                        FileSizeFormattedEn = d.FileSizeFormattedEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of multilingual file properties
                        CategoryUz = d.CategoryUz,
                        CategoryRu = d.CategoryRu,
                        CategoryEn = d.CategoryEn,
                        Status = d.Status.ToString(),
                        PublishDate = d.PublishDate
                    })
                    .FirstOrDefaultAsync();

                if (document == null)
                {
                    return NotFound(new { message = "Document not found or not published." });
                }

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the document.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get latest documents (for homepage/featured sections)
        /// </summary>
        /// <param name="count">Number of latest documents to return (default: 5, max: 20)</param>
        /// <returns>Latest published documents</returns>
        [HttpGet("latest")]
        public async Task<ActionResult<List<DocumentItemDto>>> GetLatestDocuments([FromQuery] int count = 5)
        {
            try
            {
                count = Math.Min(20, Math.Max(1, count));

                var latestDocuments = await _context.Documents
                    .Where(d => d.Status == DocumentStatus.Published)
                    .OrderByDescending(d => d.PublishDate)
                    .Take(count)
                    .Select(d => new DocumentItemDto
                    {
                        Id = d.Id,
                        TitleUz = d.TitleUz,
                        TitleRu = d.TitleRu,
                        TitleEn = d.TitleEn,
                        DescriptionUz = TruncateText(d.DescriptionUz, 150),
                        DescriptionRu = TruncateText(d.DescriptionRu, 150),
                        DescriptionEn = TruncateText(d.DescriptionEn, 150),
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added multilingual file properties
                        FileUrl = d.FileUrl, // Russian (keeping original)
                        FileName = d.FileName,
                        FileSizeBytes = d.FileSizeBytes,
                        FileSizeFormatted = d.FileSizeFormatted,
                        FileUrlUz = d.FileUrlUz, // Uzbek
                        FileNameUz = d.FileNameUz,
                        FileSizeBytesUz = d.FileSizeBytesUz,
                        FileSizeFormattedUz = d.FileSizeFormattedUz,
                        FileUrlEn = d.FileUrlEn, // English
                        FileNameEn = d.FileNameEn,
                        FileSizeBytesEn = d.FileSizeBytesEn,
                        FileSizeFormattedEn = d.FileSizeFormattedEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of multilingual file properties
                        CategoryUz = d.CategoryUz,
                        CategoryRu = d.CategoryRu,
                        CategoryEn = d.CategoryEn,
                        PublishDate = d.PublishDate
                    })
                    .ToListAsync();

                return Ok(latestDocuments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching latest documents.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available document categories
        /// </summary>
        /// <returns>List of available categories with translations</returns>
        [HttpGet("categories")]
        public ActionResult<List<DocumentCategoryDto>> GetCategories()
        {
            try
            {
                var categories = DocumentCategoryExtensions.GetAllCategories()
                    .Select(c => new DocumentCategoryDto
                    {
                        Uz = c.Uz,
                        Ru = c.Ru,
                        En = c.En
                    })
                    .ToList();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching categories.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get documents by category
        /// </summary>
        /// <param name="category">Category name (in any language)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of documents for the specified category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<DocumentListDto>> GetDocumentsByCategory(
            string category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetDocuments(page, pageSize, category);
        }

        /// <summary>
        /// Get document statistics
        /// </summary>
        /// <returns>Various statistics about documents</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetDocumentStatistics()
        {
            try
            {
                var now = DateTime.UtcNow;
                var thisMonth = new DateTime(now.Year, now.Month, 1);

                var stats = new
                {
                    TotalDocuments = await _context.Documents.CountAsync(),
                    PublishedDocuments = await _context.Documents
                        .CountAsync(d => d.Status == DocumentStatus.Published),
                    UnpublishedDocuments = await _context.Documents
                        .CountAsync(d => d.Status == DocumentStatus.Unpublished),
                    DocumentsCategory = await _context.Documents
                        .CountAsync(d => d.Category == DocumentCategory.Documents && d.Status == DocumentStatus.Published),
                    SecurityAndValuesCategory = await _context.Documents
                        .CountAsync(d => d.Category == DocumentCategory.SecurityAndValues && d.Status == DocumentStatus.Published),
                    ThisMonthDocuments = await _context.Documents
                        .CountAsync(d => d.CreatedAt >= thisMonth),
                    RecentDocuments = await _context.Documents
                        .CountAsync(d => d.PublishDate >= now.AddDays(-30) && d.Status == DocumentStatus.Published)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching statistics.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all document summaries for sitemap generation
        /// </summary>
        /// <returns>List of all published document summaries</returns>
        [HttpGet("summaries")]
        public async Task<ActionResult<List<DocumentSummaryDto>>> GetDocumentSummaries()
        {
            try
            {
                var summaries = await _context.Documents
                    .Where(d => d.Status == DocumentStatus.Published)
                    .Select(d => new DocumentSummaryDto
                    {
                        Id = d.Id,
                        TitleUz = d.TitleUz,
                        TitleRu = d.TitleRu,
                        TitleEn = d.TitleEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added multilingual file properties
                        FileUrl = d.FileUrl, // Russian (keeping original)
                        FileName = d.FileName,
                        FileSizeFormatted = d.FileSizeFormatted,
                        FileUrlUz = d.FileUrlUz, // Uzbek
                        FileNameUz = d.FileNameUz,
                        FileSizeFormattedUz = d.FileSizeFormattedUz,
                        FileUrlEn = d.FileUrlEn, // English
                        FileNameEn = d.FileNameEn,
                        FileSizeFormattedEn = d.FileSizeFormattedEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of multilingual file properties
                        PublishDate = d.PublishDate,
                        Status = d.Status.ToString()
                    })
                    .ToListAsync();

                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching summaries.", error = ex.Message });
            }
        }

        /// <summary>
        /// Search documents by keyword
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        public async Task<ActionResult<DocumentListDto>> SearchDocuments(
            [FromQuery] string keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Search keyword is required." });
            }

            return await GetDocuments(page, pageSize, search: keyword);
        }

        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added new endpoint for downloading specific language documents
        /// <summary>
        /// Download a document file by language
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="language">Language code (uz, ru, en) - optional, defaults to 'en'</param>
        /// <returns>File download</returns>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id, [FromQuery] string language = "en")
        {
            try
            {
                var document = await _context.Documents
                    .Where(d => d.Id == id && d.Status == DocumentStatus.Published)
                    .FirstOrDefaultAsync();

                if (document == null)
                {
                    return NotFound(new { message = "Document not found or not published." });
                }

                // Get the appropriate file URL based on language
                string fileUrl = language.ToLower() switch
                {
                    "uz" => document.FileUrlUz,
                    "ru" => document.FileUrl,
                    "en" => document.FileUrlEn,
                    _ => document.FileUrlEn // Default to English
                };

                if (string.IsNullOrEmpty(fileUrl))
                {
                    return NotFound(new { message = $"Document not available in {language.ToUpper()} language." });
                }

                // For this example, we'll redirect to the file URL
                // In a production environment, you might want to serve the file directly
                // and log download statistics
                return Redirect(fileUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the document.", error = ex.Message });
            }
        }

        /// <summary>
        /// Download a document file (legacy endpoint - defaults to Russian)
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>File download</returns>
        [HttpGet("{id}/download/legacy")]
        public async Task<IActionResult> DownloadDocumentLegacy(int id)
        {
            return await DownloadDocument(id, "ru");
        }
        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of new download endpoints

        /// <summary>
        /// Helper method to truncate text content
        /// </summary>
        private static string? TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            var truncated = text.Substring(0, maxLength).Trim();
            var lastSpace = truncated.LastIndexOf(' ');

            if (lastSpace > 0)
                truncated = truncated.Substring(0, lastSpace);

            return truncated + "...";
        }
    }
}