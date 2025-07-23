// Soskd.Admin/Controllers/VacancyApplicationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;
using Soskd.Admin.Services;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class VacancyApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IApiFileService _apiFileService;
        private readonly ILogger<VacancyApplicationsController> _logger;

        public VacancyApplicationsController(
            ApplicationDbContext context,
            IFileService fileService,
            IApiFileService apiFileService,
            ILogger<VacancyApplicationsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _apiFileService = apiFileService;
            _logger = logger;
        }

        // GET: VacancyApplications
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, int? vacancyId = null)
        {
            try
            {
                _logger.LogInformation("Loading vacancy applications - Page: {Page}, VacancyId: {VacancyId}", page, vacancyId);

                page = Math.Max(1, page);
                pageSize = Math.Min(50, Math.Max(1, pageSize));

                var query = _context.VacancyApplications.AsQueryable();

                // Apply vacancy filter
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
                    .Select(a => new VacancyApplicationItemViewModel
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        Phone = a.Phone,
                        ShortCoverLetter = a.CoverLetter.Length > 150 ? a.CoverLetter.Substring(0, 150) + "..." : a.CoverLetter,
                        ResumeUrl = a.ResumeUrl,
                        ResumeFileName = a.ResumeFileName,
                        VacancyId = a.VacancyId,
                        VacancyTitle = a.VacancyTitle,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                // Get vacancy filters (vacancies with applications)
                var vacancyFilters = await _context.VacancyApplications
                    .GroupBy(a => new { a.VacancyId, a.VacancyTitle })
                    .Select(g => new VacancyFilterOption
                    {
                        Id = g.Key.VacancyId,
                        Title = g.Key.VacancyTitle,
                        ApplicationCount = g.Count()
                    })
                    .OrderBy(v => v.Title)
                    .ToListAsync();

                var selectedVacancyTitle = vacancyId.HasValue
                    ? vacancyFilters.FirstOrDefault(v => v.Id == vacancyId.Value)?.Title
                    : null;

                var viewModel = new VacancyApplicationListViewModel
                {
                    Applications = applications,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    SelectedVacancyId = vacancyId,
                    SelectedVacancyTitle = selectedVacancyTitle,
                    VacancyFilters = vacancyFilters
                };

                ViewBag.ApiFileService = _apiFileService;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vacancy applications");
                TempData["ErrorMessage"] = $"Error loading applications: {ex.Message}";
                return View(new VacancyApplicationListViewModel());
            }
        }

        // GET: VacancyApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var application = await _context.VacancyApplications
                    .Where(a => a.Id == id)
                    .Select(a => new VacancyApplicationViewModel
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
                    return NotFound();
                }

                ViewBag.ApiFileService = _apiFileService;
                return View(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading application details for ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error loading application details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: VacancyApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var application = await _context.VacancyApplications
                    .Where(a => a.Id == id)
                    .Select(a => new VacancyApplicationViewModel
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
                    return NotFound();
                }

                ViewBag.ApiFileService = _apiFileService;
                return View(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading application for deletion: {Id}", id);
                TempData["ErrorMessage"] = $"Error loading application: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: VacancyApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var application = await _context.VacancyApplications.FindAsync(id);
                if (application != null)
                {
                    _logger.LogInformation("Deleting vacancy application {Id} and associated resume file", id);

                    // Delete resume file from API storage
                    if (!string.IsNullOrEmpty(application.ResumeUrl))
                    {
                        try
                        {
                            await _apiFileService.DeleteFileAsync(application.ResumeUrl);
                            _logger.LogInformation("Resume file deleted successfully: {ResumeUrl}", application.ResumeUrl);
                        }
                        catch (Exception fileEx)
                        {
                            _logger.LogWarning(fileEx, "Failed to delete resume file: {ResumeUrl}", application.ResumeUrl);
                            // Continue with deletion even if file deletion fails
                        }
                    }

                    // Delete application record
                    _context.VacancyApplications.Remove(application);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Application from {application.FirstName} {application.LastName} deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Application not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vacancy application {Id}", id);
                TempData["ErrorMessage"] = $"Error deleting application: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Download resume file
        public async Task<IActionResult> DownloadResume(int id)
        {
            try
            {
                var application = await _context.VacancyApplications
                    .Where(a => a.Id == id)
                    .Select(a => new { a.ResumeUrl, a.ResumeFileName })
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    return NotFound();
                }

                // For API-based file service, redirect to the API file URL
                if (!string.IsNullOrEmpty(application.ResumeUrl))
                {
                    var fileUrl = _apiFileService.GetApiFileUrl(application.ResumeUrl);
                    return Redirect(fileUrl);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading resume for application {Id}", id);
                TempData["ErrorMessage"] = "Error downloading resume file.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}