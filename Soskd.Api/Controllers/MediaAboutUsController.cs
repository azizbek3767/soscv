// Soskd.Api/Controllers/MediaAboutUsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaAboutUsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MediaAboutUsController> _logger;

        public MediaAboutUsController(ApplicationDbContext context, ILogger<MediaAboutUsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of published media items
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="language">Language filter (uz, ru, en)</param>
        /// <returns>Paginated list of media items</returns>
        [HttpGet]
        public async Task<ActionResult<MediaAboutUsListDto>> GetMediaAboutUs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? language = null)
        {
            try
            {
                _logger.LogInformation("Getting media about us list - Page: {Page}, PageSize: {PageSize}, Language: {Language}",
                    page, pageSize, language);

                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                // Build query for published media items only
                var query = _context.MediaAboutUs
                    .Where(m => m.Status == MediaStatus.Published)
                    .Where(m => m.PublishDate <= DateTime.UtcNow)
                    .OrderByDescending(m => m.PublishDate);

                // Get total count
                var totalCount = await query.CountAsync();

                // Get paginated items
                var mediaItems = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MediaAboutUsItemDto
                    {
                        Id = m.Id,
                        TitleUz = m.TitleUz,
                        TitleRu = m.TitleRu,
                        TitleEn = m.TitleEn,
                        DescriptionUz = TruncateHtml(m.DescriptionUz, 200),
                        DescriptionRu = TruncateHtml(m.DescriptionRu, 200),
                        DescriptionEn = TruncateHtml(m.DescriptionEn, 200),
                        PhotoUrl = m.PhotoUrl,
                        SourceLink = m.SourceLink,
                        PublishDate = m.PublishDate
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new MediaAboutUsListDto
                {
                    MediaItems = mediaItems,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                _logger.LogInformation("Successfully retrieved {Count} media items", mediaItems.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media about us list");
                return StatusCode(500, new { message = "An error occurred while retrieving media items" });
            }
        }

        /// <summary>
        /// Get specific media item by ID
        /// </summary>
        /// <param name="id">Media item ID</param>
        /// <returns>Media item details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MediaAboutUsDto>> GetMediaAboutUsById(int id)
        {
            try
            {
                _logger.LogInformation("Getting media about us item by ID: {Id}", id);

                var mediaItem = await _context.MediaAboutUs
                    .Where(m => m.Id == id && m.Status == MediaStatus.Published)
                    .Where(m => m.PublishDate <= DateTime.UtcNow)
                    .Select(m => new MediaAboutUsDto
                    {
                        Id = m.Id,
                        TitleUz = m.TitleUz,
                        TitleRu = m.TitleRu,
                        TitleEn = m.TitleEn,
                        DescriptionUz = m.DescriptionUz,
                        DescriptionRu = m.DescriptionRu,
                        DescriptionEn = m.DescriptionEn,
                        PhotoUrl = m.PhotoUrl,
                        SourceLink = m.SourceLink,
                        Status = m.Status.ToString(),
                        PublishDate = m.PublishDate
                    })
                    .FirstOrDefaultAsync();

                if (mediaItem == null)
                {
                    _logger.LogWarning("Media about us item not found with ID: {Id}", id);
                    return NotFound(new { message = "Media item not found" });
                }

                _logger.LogInformation("Successfully retrieved media about us item: {Title}", mediaItem.TitleEn);
                return Ok(mediaItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media about us item by ID: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the media item" });
            }
        }

        /// <summary>
        /// Get latest media items (for homepage/widgets)
        /// </summary>
        /// <param name="count">Number of items to return (default: 5, max: 20)</param>
        /// <returns>Latest media items</returns>
        [HttpGet("latest")]
        public async Task<ActionResult<List<MediaAboutUsItemDto>>> GetLatestMediaAboutUs([FromQuery] int count = 5)
        {
            try
            {
                _logger.LogInformation("Getting latest media about us items - Count: {Count}", count);

                // Validate count
                count = Math.Min(20, Math.Max(1, count));

                var latestMediaItems = await _context.MediaAboutUs
                    .Where(m => m.Status == MediaStatus.Published)
                    .Where(m => m.PublishDate <= DateTime.UtcNow)
                    .OrderByDescending(m => m.PublishDate)
                    .Take(count)
                    .Select(m => new MediaAboutUsItemDto
                    {
                        Id = m.Id,
                        TitleUz = m.TitleUz,
                        TitleRu = m.TitleRu,
                        TitleEn = m.TitleEn,
                        DescriptionUz = TruncateHtml(m.DescriptionUz, 150),
                        DescriptionRu = TruncateHtml(m.DescriptionRu, 150),
                        DescriptionEn = TruncateHtml(m.DescriptionEn, 150),
                        PhotoUrl = m.PhotoUrl,
                        SourceLink = m.SourceLink,
                        PublishDate = m.PublishDate
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} latest media items", latestMediaItems.Count);
                return Ok(latestMediaItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest media about us items");
                return StatusCode(500, new { message = "An error occurred while retrieving latest media items" });
            }
        }

        /// <summary>
        /// Search media items
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="language">Language for search (uz, ru, en, or all)</param>
        /// <returns>Search results</returns>
        [HttpGet("search")]
        public async Task<ActionResult<MediaAboutUsListDto>> SearchMediaAboutUs(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string language = "all")
        {
            try
            {
                _logger.LogInformation("Searching media about us - Query: {Query}, Page: {Page}, PageSize: {PageSize}, Language: {Language}",
                    query, page, pageSize, language);

                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query is required" });
                }

                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var searchQuery = _context.MediaAboutUs
                    .Where(m => m.Status == MediaStatus.Published)
                    .Where(m => m.PublishDate <= DateTime.UtcNow);

                // Apply search filters based on language
                if (language.ToLower() == "uz")
                {
                    searchQuery = searchQuery.Where(m =>
                        m.TitleUz.Contains(query) || m.DescriptionUz.Contains(query));
                }
                else if (language.ToLower() == "ru")
                {
                    searchQuery = searchQuery.Where(m =>
                        m.TitleRu.Contains(query) || m.DescriptionRu.Contains(query));
                }
                else if (language.ToLower() == "en")
                {
                    searchQuery = searchQuery.Where(m =>
                        m.TitleEn.Contains(query) || m.DescriptionEn.Contains(query));
                }
                else // search all languages
                {
                    searchQuery = searchQuery.Where(m =>
                        m.TitleUz.Contains(query) || m.DescriptionUz.Contains(query) ||
                        m.TitleRu.Contains(query) || m.DescriptionRu.Contains(query) ||
                        m.TitleEn.Contains(query) || m.DescriptionEn.Contains(query));
                }

                searchQuery = searchQuery.OrderByDescending(m => m.PublishDate);

                // Get total count
                var totalCount = await searchQuery.CountAsync();

                // Get paginated results
                var mediaItems = await searchQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MediaAboutUsItemDto
                    {
                        Id = m.Id,
                        TitleUz = m.TitleUz,
                        TitleRu = m.TitleRu,
                        TitleEn = m.TitleEn,
                        DescriptionUz = TruncateHtml(m.DescriptionUz, 200),
                        DescriptionRu = TruncateHtml(m.DescriptionRu, 200),
                        DescriptionEn = TruncateHtml(m.DescriptionEn, 200),
                        PhotoUrl = m.PhotoUrl,
                        SourceLink = m.SourceLink,
                        PublishDate = m.PublishDate
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new MediaAboutUsListDto
                {
                    MediaItems = mediaItems,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                _logger.LogInformation("Search completed - Found {Count} media items", totalCount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching media about us");
                return StatusCode(500, new { message = "An error occurred while searching media items" });
            }
        }

        /// <summary>
        /// Get media statistics
        /// </summary>
        /// <returns>Media statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetMediaStatistics()
        {
            try
            {
                _logger.LogInformation("Getting media about us statistics");

                var now = DateTime.UtcNow;
                var currentMonth = new DateTime(now.Year, now.Month, 1);

                var stats = new
                {
                    TotalMedia = await _context.MediaAboutUs.CountAsync(),
                    PublishedMedia = await _context.MediaAboutUs
                        .CountAsync(m => m.Status == MediaStatus.Published),
                    UnpublishedMedia = await _context.MediaAboutUs
                        .CountAsync(m => m.Status == MediaStatus.Unpublished),
                    ThisMonthMedia = await _context.MediaAboutUs
                        .CountAsync(m => m.CreatedAt >= currentMonth),
                    RecentMedia = await _context.MediaAboutUs
                        .CountAsync(m => m.PublishDate >= now.AddDays(-30) &&
                                        m.Status == MediaStatus.Published)
                };

                _logger.LogInformation("Successfully retrieved media statistics");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving statistics" });
            }
        }

        /// <summary>
        /// Helper method to truncate HTML content
        /// </summary>
        /// <param name="html">HTML content</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Truncated content</returns>
        private static string TruncateHtml(string html, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Simple HTML tag removal for truncation
            var plainText = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);

            if (plainText.Length <= maxLength)
                return html;

            // Find a good place to truncate (avoid cutting words)
            var truncateAt = maxLength;
            var lastSpace = plainText.LastIndexOf(' ', maxLength);
            if (lastSpace > maxLength * 0.8) // If last space is reasonably close to max length
            {
                truncateAt = lastSpace;
            }

            return html.Length > truncateAt ? html.Substring(0, truncateAt) + "..." : html;
        }
    }
}