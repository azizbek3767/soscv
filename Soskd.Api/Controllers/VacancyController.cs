using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Infrastructure.Data;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacanciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VacanciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all published and active vacancies with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="includeExpired">Include expired vacancies (default: false)</param>
        /// <param name="search">Search in titles and descriptions (optional)</param>
        /// <returns>Paginated list of published vacancies</returns>
        [HttpGet]
        public async Task<ActionResult<VacancyListDto>> GetVacancies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeExpired = false,
            [FromQuery] string? search = null)
        {
            try
            {
                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var query = _context.Vacancies
                    .Where(v => v.PublishedDate.HasValue && v.PublishedDate.Value <= DateTime.UtcNow)
                    .AsQueryable();

                // Filter out expired vacancies unless requested
                if (!includeExpired)
                {
                    query = query.Where(v => !v.Deadline.HasValue || v.Deadline.Value >= DateTime.UtcNow);
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(v => v.TitleEn.ToLower().Contains(searchLower) ||
                                           v.TitleUz.ToLower().Contains(searchLower) ||
                                           v.TitleRu.ToLower().Contains(searchLower) ||
                                           v.DescriptionEn.ToLower().Contains(searchLower) ||
                                           v.DescriptionUz.ToLower().Contains(searchLower) ||
                                           v.DescriptionRu.ToLower().Contains(searchLower));
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var vacancies = await query
                    .OrderByDescending(v => v.PublishedDate)
                    .ThenByDescending(v => v.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(v => new VacancyItemDto
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        DescriptionUz = TruncateHtml(v.DescriptionUz, 200),
                        DescriptionRu = TruncateHtml(v.DescriptionRu, 200),
                        DescriptionEn = TruncateHtml(v.DescriptionEn, 200),
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync();

                var result = new VacancyListDto
                {
                    Vacancies = vacancies,
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
                return StatusCode(500, new { message = "An error occurred while fetching vacancies.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific vacancy by ID
        /// </summary>
        /// <param name="id">Vacancy ID</param>
        /// <returns>Vacancy details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyDto>> GetVacancy(int id)
        {
            try
            {
                var vacancy = await _context.Vacancies
                    .Where(v => v.Id == id &&
                               v.PublishedDate.HasValue &&
                               v.PublishedDate.Value <= DateTime.UtcNow)
                    .Select(v => new VacancyDto
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        DescriptionUz = v.DescriptionUz,
                        DescriptionRu = v.DescriptionRu,
                        DescriptionEn = v.DescriptionEn,
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        CreatedAt = v.CreatedAt,
                        UpdatedAt = v.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (vacancy == null)
                {
                    return NotFound(new { message = "Vacancy not found or not published." });
                }

                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the vacancy.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get latest active vacancies (for homepage/featured sections)
        /// </summary>
        /// <param name="count">Number of latest vacancies to return (default: 5, max: 20)</param>
        /// <returns>Latest published and active vacancies</returns>
        [HttpGet("latest")]
        public async Task<ActionResult<List<VacancyItemDto>>> GetLatestVacancies([FromQuery] int count = 5)
        {
            try
            {
                count = Math.Min(20, Math.Max(1, count));

                var latestVacancies = await _context.Vacancies
                    .Where(v => v.PublishedDate.HasValue &&
                               v.PublishedDate.Value <= DateTime.UtcNow &&
                               (!v.Deadline.HasValue || v.Deadline.Value >= DateTime.UtcNow))
                    .OrderByDescending(v => v.PublishedDate)
                    .Take(count)
                    .Select(v => new VacancyItemDto
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        DescriptionUz = TruncateHtml(v.DescriptionUz, 150),
                        DescriptionRu = TruncateHtml(v.DescriptionRu, 150),
                        DescriptionEn = TruncateHtml(v.DescriptionEn, 150),
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync();

                return Ok(latestVacancies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching latest vacancies.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active vacancies (published and not expired)
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of active vacancies</returns>
        [HttpGet("active")]
        public async Task<ActionResult<VacancyListDto>> GetActiveVacancies(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetVacancies(page, pageSize, includeExpired: false);
        }

        /// <summary>
        /// Get vacancy statistics
        /// </summary>
        /// <returns>Various statistics about vacancies</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetVacancyStatistics()
        {
            try
            {
                var now = DateTime.UtcNow;
                var thisMonth = new DateTime(now.Year, now.Month, 1);

                var stats = new
                {
                    TotalVacancies = await _context.Vacancies.CountAsync(),
                    PublishedVacancies = await _context.Vacancies
                        .CountAsync(v => v.PublishedDate.HasValue && v.PublishedDate.Value <= now),
                    ActiveVacancies = await _context.Vacancies
                        .CountAsync(v => v.PublishedDate.HasValue &&
                                       v.PublishedDate.Value <= now &&
                                       (!v.Deadline.HasValue || v.Deadline.Value >= now)),
                    ExpiredVacancies = await _context.Vacancies
                        .CountAsync(v => v.PublishedDate.HasValue &&
                                       v.PublishedDate.Value <= now &&
                                       v.Deadline.HasValue && v.Deadline.Value < now),
                    DraftVacancies = await _context.Vacancies
                        .CountAsync(v => !v.PublishedDate.HasValue || v.PublishedDate.Value > now),
                    ThisMonthVacancies = await _context.Vacancies
                        .CountAsync(v => v.CreatedAt >= thisMonth),
                    ExpiringThisWeek = await _context.Vacancies
                        .CountAsync(v => v.PublishedDate.HasValue &&
                                       v.PublishedDate.Value <= now &&
                                       v.Deadline.HasValue &&
                                       v.Deadline.Value >= now &&
                                       v.Deadline.Value <= now.AddDays(7))
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching statistics.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all vacancy summaries for sitemap generation
        /// </summary>
        /// <returns>List of all published vacancy summaries</returns>
        [HttpGet("summaries")]
        public async Task<ActionResult<List<VacancySummaryDto>>> GetVacancySummaries()
        {
            try
            {
                var summaries = await _context.Vacancies
                    .Where(v => v.PublishedDate.HasValue && v.PublishedDate.Value <= DateTime.UtcNow)
                    .Select(v => new VacancySummaryDto
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        Status = !v.PublishedDate.HasValue || v.PublishedDate.Value > DateTime.UtcNow ? "Draft" :
                                v.Deadline.HasValue && v.Deadline.Value < DateTime.UtcNow ? "Expired" : "Active"
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
        /// Search vacancies by keyword
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        public async Task<ActionResult<VacancyListDto>> SearchVacancies(
            [FromQuery] string keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Search keyword is required." });
            }

            return await GetVacancies(page, pageSize, includeExpired: false, search: keyword);
        }

        /// <summary>
        /// Get vacancies expiring soon
        /// </summary>
        /// <param name="days">Number of days to look ahead (default: 7)</param>
        /// <returns>List of vacancies expiring within the specified days</returns>
        [HttpGet("expiring-soon")]
        public async Task<ActionResult<List<VacancyItemDto>>> GetVacanciesExpiringSoon([FromQuery] int days = 7)
        {
            try
            {
                days = Math.Max(1, Math.Min(30, days)); // Limit between 1 and 30 days

                var now = DateTime.UtcNow;
                var expiringVacancies = await _context.Vacancies
                    .Where(v => v.PublishedDate.HasValue &&
                               v.PublishedDate.Value <= now &&
                               v.Deadline.HasValue &&
                               v.Deadline.Value >= now &&
                               v.Deadline.Value <= now.AddDays(days))
                    .OrderBy(v => v.Deadline)
                    .Select(v => new VacancyItemDto
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        DescriptionUz = TruncateHtml(v.DescriptionUz, 150),
                        DescriptionRu = TruncateHtml(v.DescriptionRu, 150),
                        DescriptionEn = TruncateHtml(v.DescriptionEn, 150),
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync();

                return Ok(expiringVacancies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching expiring vacancies.", error = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to truncate HTML content
        /// </summary>
        private static string TruncateHtml(string html, int maxLength)
        {
            if (string.IsNullOrEmpty(html) || html.Length <= maxLength)
                return html;

            // Remove HTML tags for length calculation
            var plainText = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);

            if (plainText.Length <= maxLength)
                return html;

            // Truncate and add ellipsis
            var truncated = plainText.Substring(0, maxLength).Trim();
            var lastSpace = truncated.LastIndexOf(' ');

            if (lastSpace > 0)
                truncated = truncated.Substring(0, lastSpace);

            return truncated + "...";
        }
    }
}