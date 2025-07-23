// Soskd.Api/Controllers/VacancyApplicationsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Api.DTOs;
using Soskd.Domain.Entities;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;

namespace Soskd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacancyApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<VacancyApplicationsController> _logger;

        public VacancyApplicationsController(
            ApplicationDbContext context,
            IFileService fileService,
            ILogger<VacancyApplicationsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Submit a new vacancy application
        /// </summary>
        /// <param name="applicationDto">Application data</param>
        /// <param name="resume">Resume file</param>
        /// <returns>Created application details</returns>
        [HttpPost]
        public async Task<ActionResult<VacancyApplicationDto>> CreateApplication(
            [FromForm] CreateVacancyApplicationDto applicationDto,
            [FromForm] IFormFile resume)
        {
            try
            {
                _logger.LogInformation("Creating new vacancy application for vacancy {VacancyId}", applicationDto.VacancyId);

                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate resume file
                if (resume == null || resume.Length == 0)
                {
                    return BadRequest(new { message = "Resume file is required." });
                }

                // Validate file size (max 5MB)
                if (resume.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Resume file size cannot exceed 5MB." });
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(resume.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Only PDF, DOC, and DOCX files are allowed for resumes." });
                }

                // Check if vacancy exists and is active
                var vacancy = await _context.Vacancies
                    .Where(v => v.Id == applicationDto.VacancyId &&
                               v.Status == Domain.Enums.VacancyStatus.Published &&
                               (v.Deadline == null || v.Deadline > DateTime.UtcNow))
                    .FirstOrDefaultAsync();

                if (vacancy == null)
                {
                    return BadRequest(new { message = "Vacancy not found or no longer accepting applications." });
                }

                // Check for duplicate application (same email for same vacancy)
                var existingApplication = await _context.VacancyApplications
                    .Where(a => a.VacancyId == applicationDto.VacancyId &&
                               a.Email.ToLower() == applicationDto.Email.ToLower())
                    .FirstOrDefaultAsync();

                if (existingApplication != null)
                {
                    return BadRequest(new { message = "You have already applied for this vacancy." });
                }

                // Save resume file
                var resumeUrl = await _fileService.SaveFileAsync(resume, "resumes");
                if (string.IsNullOrEmpty(resumeUrl))
                {
                    return StatusCode(500, new { message = "Failed to save resume file." });
                }

                // Create application entity
                var application = new VacancyApplication
                {
                    FirstName = applicationDto.FirstName.Trim(),
                    LastName = applicationDto.LastName.Trim(),
                    Email = applicationDto.Email.Trim().ToLowerInvariant(),
                    Phone = applicationDto.Phone?.Trim(),
                    CoverLetter = applicationDto.CoverLetter.Trim(),
                    ResumeUrl = resumeUrl,
                    ResumeFileName = resume.FileName,
                    VacancyId = applicationDto.VacancyId,
                    VacancyTitle = applicationDto.VacancyTitle.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.VacancyApplications.Add(application);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vacancy application created successfully with ID {ApplicationId}", application.Id);

                // Return created application
                var result = new VacancyApplicationDto
                {
                    Id = application.Id,
                    FirstName = application.FirstName,
                    LastName = application.LastName,
                    Email = application.Email,
                    Phone = application.Phone,
                    CoverLetter = application.CoverLetter,
                    ResumeUrl = application.ResumeUrl,
                    ResumeFileName = application.ResumeFileName,
                    VacancyId = application.VacancyId,
                    VacancyTitle = application.VacancyTitle,
                    CreatedAt = application.CreatedAt
                };

                return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vacancy application");
                return StatusCode(500, new { message = "An error occurred while submitting your application. Please try again." });
            }
        }

        /// <summary>
        /// Get a specific application by ID
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Application details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VacancyApplicationDto>> GetApplication(int id)
        {
            try
            {
                var application = await _context.VacancyApplications
                    .Where(a => a.Id == id)
                    .Select(a => new VacancyApplicationDto
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        Phone = a.Phone,
                        CoverLetter = a.CoverLetter,
                        ResumeUrl = a.ResumeUrl,
                        ResumeFileName = a.ResumeFileName,
                        VacancyId = a.VacancyId,
                        VacancyTitle = a.VacancyTitle,
                        CreatedAt = a.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    return NotFound(new { message = "Application not found." });
                }

                return Ok(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application {ApplicationId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the application." });
            }
        }

        /// <summary>
        /// Get all applications with pagination (for admin use)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="vacancyId">Filter by vacancy ID</param>
        /// <returns>Paginated list of applications</returns>
        [HttpGet]
        public async Task<ActionResult<VacancyApplicationListDto>> GetApplications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? vacancyId = null)
        {
            try
            {
                // Validate parameters
                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var query = _context.VacancyApplications.AsQueryable();

                // Apply vacancy filter if provided
                if (vacancyId.HasValue)
                {
                    query = query.Where(a => a.VacancyId == vacancyId.Value);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var applications = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new VacancyApplicationDto
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        Phone = a.Phone,
                        CoverLetter = a.CoverLetter.Length > 200 ? a.CoverLetter.Substring(0, 200) + "..." : a.CoverLetter,
                        ResumeUrl = a.ResumeUrl,
                        ResumeFileName = a.ResumeFileName,
                        VacancyId = a.VacancyId,
                        VacancyTitle = a.VacancyTitle,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                var result = new VacancyApplicationListDto
                {
                    Applications = applications,
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
                _logger.LogError(ex, "Error retrieving applications");
                return StatusCode(500, new { message = "An error occurred while retrieving applications." });
            }
        }

        /// <summary>
        /// Delete an application (admin only)
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            try
            {
                var application = await _context.VacancyApplications.FindAsync(id);
                if (application == null)
                {
                    return NotFound(new { message = "Application not found." });
                }

                // Delete resume file
                if (!string.IsNullOrEmpty(application.ResumeUrl))
                {
                    await _fileService.DeleteFileAsync(application.ResumeUrl);
                }

                // Delete application record
                _context.VacancyApplications.Remove(application);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vacancy application {ApplicationId} deleted successfully", id);

                return Ok(new { message = "Application deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting application {ApplicationId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the application." });
            }
        }
    }
}