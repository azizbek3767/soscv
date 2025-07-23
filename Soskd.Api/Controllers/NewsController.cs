// Soskd.Api/Controllers/NewsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Soskd.Api.DTOs;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all published news with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="category">Filter by category (optional)</param>
        /// <param name="search">Search in titles and descriptions (optional)</param>
        /// <returns>Paginated list of published news</returns>
        [HttpGet]
        public async Task<ActionResult<NewsListDto>> GetNews(
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

                var query = _context.News
                    .Where(n => n.Status == NewsStatus.Published)
                    .AsQueryable();

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(n => n.CategoryEn == category ||
                                           n.CategoryUz == category ||
                                           n.CategoryRu == category);
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(n => n.TitleEn.ToLower().Contains(searchLower) ||
                                           n.TitleUz.ToLower().Contains(searchLower) ||
                                           n.TitleRu.ToLower().Contains(searchLower) ||
                                           n.DescriptionEn.ToLower().Contains(searchLower) ||
                                           n.DescriptionUz.ToLower().Contains(searchLower) ||
                                           n.DescriptionRu.ToLower().Contains(searchLower));
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var news = await query
                    .OrderByDescending(n => n.PublishedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new NewsItemDto
                    {
                        Id = n.Id,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn,
                        DescriptionUz = TruncateHtml(n.DescriptionUz, 200),
                        DescriptionRu = TruncateHtml(n.DescriptionRu, 200),
                        DescriptionEn = TruncateHtml(n.DescriptionEn, 200),
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        MetaTitleUz = n.MetaTitleUz,
                        MetaTitleRu = n.MetaTitleRu,
                        MetaTitleEn = n.MetaTitleEn,
                        MetaDescriptionUz = n.MetaDescriptionUz,
                        MetaDescriptionRu = n.MetaDescriptionRu,
                        MetaDescriptionEn = n.MetaDescriptionEn,
                        CategoryUz = n.CategoryUz,
                        CategoryRu = n.CategoryRu,
                        CategoryEn = n.CategoryEn,
                        SmallPhotoUrl = n.SmallPhotoUrl,
                        LargePhotoUrl = n.LargePhotoUrl,
                        PublishedDate = n.PublishedDate
                    })
                    .ToListAsync();

                var result = new NewsListDto
                {
                    News = news,
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
                return StatusCode(500, new { message = "An error occurred while fetching news.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific news article by ID
        /// </summary>
        /// <param name="id">News ID</param>
        /// <returns>News details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetNews(int id)
        {
            try
            {
                var news = await _context.News
                    .Where(n => n.Id == id && n.Status == NewsStatus.Published)
                    .Select(n => new NewsDto
                    {
                        Id = n.Id,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn,
                        DescriptionUz = n.DescriptionUz,
                        DescriptionRu = n.DescriptionRu,
                        DescriptionEn = n.DescriptionEn,
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        MetaTitleUz = n.MetaTitleUz,
                        MetaTitleRu = n.MetaTitleRu,
                        MetaTitleEn = n.MetaTitleEn,
                        MetaDescriptionUz = n.MetaDescriptionUz,
                        MetaDescriptionRu = n.MetaDescriptionRu,
                        MetaDescriptionEn = n.MetaDescriptionEn,
                        CategoryUz = n.CategoryUz,
                        CategoryRu = n.CategoryRu,
                        CategoryEn = n.CategoryEn,
                        Status = n.Status.ToString(),
                        SmallPhotoUrl = n.SmallPhotoUrl,
                        LargePhotoUrl = n.LargePhotoUrl,
                        PublishedDate = n.PublishedDate
                    })
                    .FirstOrDefaultAsync();

                if (news == null)
                {
                    return NotFound(new { message = "News not found or not published." });
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the news.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get news by slug (SEO-friendly URLs)
        /// </summary>
        /// <param name="slug">News slug in any language</param>
        /// <returns>News details</returns>
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<NewsDto>> GetNewsBySlug(string slug)
        {
            try
            {
                var news = await _context.News
                    .Where(n => (n.SlugUz == slug || n.SlugRu == slug || n.SlugEn == slug) &&
                               n.Status == NewsStatus.Published)
                    .Select(n => new NewsDto
                    {
                        Id = n.Id,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn,
                        DescriptionUz = n.DescriptionUz,
                        DescriptionRu = n.DescriptionRu,
                        DescriptionEn = n.DescriptionEn,
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        MetaTitleUz = n.MetaTitleUz,
                        MetaTitleRu = n.MetaTitleRu,
                        MetaTitleEn = n.MetaTitleEn,
                        MetaDescriptionUz = n.MetaDescriptionUz,
                        MetaDescriptionRu = n.MetaDescriptionRu,
                        MetaDescriptionEn = n.MetaDescriptionEn,
                        CategoryUz = n.CategoryUz,
                        CategoryRu = n.CategoryRu,
                        CategoryEn = n.CategoryEn,
                        Status = n.Status.ToString(),
                        SmallPhotoUrl = n.SmallPhotoUrl,
                        LargePhotoUrl = n.LargePhotoUrl,
                        PublishedDate = n.PublishedDate
                    })
                    .FirstOrDefaultAsync();

                if (news == null)
                {
                    return NotFound(new { message = "News not found or not published." });
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the news.", error = ex.Message });
            }
        }


        /// <summary>
        /// Get latest news (for homepage/featured sections)
        /// </summary>
        /// <param name="count">Number of latest news to return (default: 5, max: 20)</param>
        /// <returns>Latest published news</returns>
        [HttpGet("latest")]
        public async Task<ActionResult<List<NewsItemDto>>> GetLatestNews([FromQuery] int count = 5)
        {
            try
            {
                count = Math.Min(20, Math.Max(1, count));

                var latestNews = await _context.News
                    .Where(n => n.Status == NewsStatus.Published)
                    .OrderByDescending(n => n.PublishedDate)
                    .Take(count)
                    .Select(n => new NewsItemDto
                    {
                        Id = n.Id,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn,
                        DescriptionUz = TruncateHtml(n.DescriptionUz, 150),
                        DescriptionRu = TruncateHtml(n.DescriptionRu, 150),
                        DescriptionEn = TruncateHtml(n.DescriptionEn, 150),
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        MetaTitleUz = n.MetaTitleUz,
                        MetaTitleRu = n.MetaTitleRu,
                        MetaTitleEn = n.MetaTitleEn,
                        MetaDescriptionUz = n.MetaDescriptionUz,
                        MetaDescriptionRu = n.MetaDescriptionRu,
                        MetaDescriptionEn = n.MetaDescriptionEn,
                        CategoryUz = n.CategoryUz,
                        CategoryRu = n.CategoryRu,
                        CategoryEn = n.CategoryEn,
                        SmallPhotoUrl = n.SmallPhotoUrl,
                        LargePhotoUrl = n.LargePhotoUrl,
                        PublishedDate = n.PublishedDate
                    })
                    .ToListAsync();

                return Ok(latestNews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching latest news.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available news categories
        /// </summary>
        /// <returns>List of available categories with translations</returns>
        [HttpGet("categories")]
        public ActionResult<List<NewsCategoryDto>> GetCategories()
        {
            try
            {
                var categories = NewsCategoryExtensions.GetAllCategories()
                    .Select(c => new NewsCategoryDto
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
        /// Get news by category
        /// </summary>
        /// <param name="category">Category name (in any language)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of news for the specified category</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<NewsListDto>> GetNewsByCategory(
            string category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await GetNews(page, pageSize, category);
        }


        /// <summary>
        /// Get all slugs for sitemap generation
        /// </summary>
        /// <returns>List of all news slugs</returns>
        [HttpGet("slugs")]
        public async Task<ActionResult<List<NewsSlugDto>>> GetNewsSlugs()
        {
            try
            {
                var slugs = await _context.News
                    .Where(n => n.Status == NewsStatus.Published)
                    .Select(n => new NewsSlugDto
                    {
                        Id = n.Id,
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn
                    })
                    .ToListAsync();

                return Ok(slugs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching slugs.", error = ex.Message });
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