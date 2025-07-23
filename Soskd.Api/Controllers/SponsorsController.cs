// Soskd.Api/Controllers/SponsorsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SponsorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all published sponsors with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <returns>Paginated list of published sponsors</returns>
        [HttpGet]
        public async Task<ActionResult<SponsorListDto>> GetSponsors(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var query = _context.Sponsors
                    .Where(s => s.Status == SponsorStatus.Published)
                    .OrderBy(s => s.DisplayOrder)
                    .ThenBy(s => s.Id);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var sponsors = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new SponsorItemDto
                    {
                        Id = s.Id,
                        TitleUz = s.TitleUz,
                        TitleRu = s.TitleRu,
                        TitleEn = s.TitleEn,
                        PhotoUrl = s.PhotoUrl,
                        Link = s.Link,
                        DisplayOrder = s.DisplayOrder
                    })
                    .ToListAsync();

                var result = new SponsorListDto
                {
                    Sponsors = sponsors,
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
                return StatusCode(500, new { message = "An error occurred while fetching sponsors.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all published sponsors (for slider/carousel display)
        /// </summary>
        /// <returns>List of all published sponsors ordered by display order</returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<SponsorItemDto>>> GetAllSponsors()
        {
            try
            {
                var sponsors = await _context.Sponsors
                    .Where(s => s.Status == SponsorStatus.Published)
                    .OrderBy(s => s.DisplayOrder)
                    .ThenBy(s => s.Id)
                    .Select(s => new SponsorItemDto
                    {
                        Id = s.Id,
                        TitleUz = s.TitleUz,
                        TitleRu = s.TitleRu,
                        TitleEn = s.TitleEn,
                        PhotoUrl = s.PhotoUrl,
                        Link = s.Link,
                        DisplayOrder = s.DisplayOrder
                    })
                    .ToListAsync();

                return Ok(sponsors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching sponsors.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific sponsor by ID
        /// </summary>
        /// <param name="id">Sponsor ID</param>
        /// <returns>Sponsor details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorDto>> GetSponsor(int id)
        {
            try
            {
                var sponsor = await _context.Sponsors
                    .Where(s => s.Id == id && s.Status == SponsorStatus.Published)
                    .Select(s => new SponsorDto
                    {
                        Id = s.Id,
                        TitleUz = s.TitleUz,
                        TitleRu = s.TitleRu,
                        TitleEn = s.TitleEn,
                        PhotoUrl = s.PhotoUrl,
                        Link = s.Link,
                        DisplayOrder = s.DisplayOrder,
                        Status = s.Status.ToString()
                    })
                    .FirstOrDefaultAsync();

                if (sponsor == null)
                {
                    return NotFound(new { message = "Sponsor not found or not published." });
                }

                return Ok(sponsor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the sponsor.", error = ex.Message });
            }
        }
    }
}