// Soskd.Admin/Controllers/SponsorsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;
using Soskd.Admin.Services;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SponsorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApiFileService _apiFileService;
        private readonly ILogger<SponsorsController> _logger;

        public SponsorsController(
            ApplicationDbContext context,
            IApiFileService apiFileService,
            ILogger<SponsorsController> logger)
        {
            _context = context;
            _apiFileService = apiFileService;
            _logger = logger;
        }

        // GET: Sponsors
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Loading sponsors index page {Page}", page);

                var totalCount = await _context.Sponsors.CountAsync();
                var sponsors = await _context.Sponsors
                    .OrderBy(s => s.DisplayOrder)
                    .ThenByDescending(s => s.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new SponsorItemViewModel
                    {
                        Id = s.Id,
                        TitleUz = s.TitleUz,
                        TitleRu = s.TitleRu,
                        TitleEn = s.TitleEn,
                        PhotoUrl = s.PhotoUrl,
                        Link = s.Link,
                        DisplayOrder = s.DisplayOrder,
                        Status = s.Status,
                        CreatedAt = s.CreatedAt,
                        UpdatedAt = s.UpdatedAt
                    })
                    .ToListAsync();

                var viewModel = new SponsorListViewModel
                {
                    Sponsors = sponsors,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                ViewBag.ApiFileService = _apiFileService;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sponsors index");
                TempData["ErrorMessage"] = $"Error loading sponsors: {ex.Message}";
                return View(new SponsorListViewModel { Sponsors = new List<SponsorItemViewModel>() });
            }
        }

        // GET: Sponsors/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Loading create sponsor page");

            var viewModel = new SponsorViewModel
            {
                Status = SponsorStatus.Published,
                DisplayOrder = 0
            };

            return View(viewModel);
        }

        // POST: Sponsors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SponsorViewModel viewModel)
        {
            _logger.LogInformation("=== CREATE SPONSOR REQUEST STARTED ===");
            _logger.LogInformation("Model State Valid: {IsValid}", ModelState.IsValid);

            // Log received data
            _logger.LogInformation("Received Data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- Link: '{Link}'", viewModel.Link ?? "NULL");
            _logger.LogInformation("- DisplayOrder: {DisplayOrder}", viewModel.DisplayOrder);
            _logger.LogInformation("- Status: {Status}", viewModel.Status);
            _logger.LogInformation("- Photo: {HasPhoto}", viewModel.Photo != null ? $"Yes ({viewModel.Photo.Length} bytes)" : "No");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid:");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning("- {Key}: {Error}", error.Key, err.ErrorMessage);
                    }
                }
                return View(viewModel);
            }

            try
            {
                _logger.LogInformation("Creating sponsor entity...");

                var currentUtc = DateTime.UtcNow;

                var sponsor = new Sponsor
                {
                    TitleUz = viewModel.TitleUz?.Trim() ?? "",
                    TitleRu = viewModel.TitleRu?.Trim() ?? "",
                    TitleEn = viewModel.TitleEn?.Trim() ?? "",
                    Link = viewModel.Link?.Trim() ?? "",
                    DisplayOrder = viewModel.DisplayOrder,
                    Status = viewModel.Status,
                    CreatedAt = currentUtc,
                    UpdatedAt = currentUtc
                };

                _logger.LogInformation("Handling photo upload via API...");
                // Handle photo upload via API (required)
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    sponsor.PhotoUrl = await _apiFileService.SaveFileAsync(viewModel.Photo, "sponsors");
                    _logger.LogInformation("Photo saved via API: {Url}", sponsor.PhotoUrl);
                }
                else
                {
                    _logger.LogError("Photo is required but not provided");
                    ModelState.AddModelError("Photo", "Logo/Photo is required.");
                    return View(viewModel);
                }

                _logger.LogInformation("Adding sponsor to context...");
                _context.Add(sponsor);

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sponsor saved with ID: {Id}", sponsor.Id);
                _logger.LogInformation("=== CREATE SPONSOR COMPLETED SUCCESSFULLY ===");

                TempData["SuccessMessage"] = "Sponsor created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sponsor");
                ModelState.AddModelError("", $"An error occurred while creating the sponsor: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: Sponsors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(m => m.Id == id);
            if (sponsor == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(sponsor);
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // GET: Sponsors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor = await _context.Sponsors.FindAsync(id);
            if (sponsor == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(sponsor);
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: Sponsors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SponsorViewModel viewModel)
        {
            _logger.LogInformation("=== EDIT SPONSOR REQUEST STARTED ===");
            _logger.LogInformation("ID: {Id}, Model State Valid: {IsValid}", id, ModelState.IsValid);

            if (id != viewModel.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} != Model ID {ModelId}", id, viewModel.Id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid:");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning("- {Key}: {Error}", error.Key, err.ErrorMessage);
                    }
                }

                ViewBag.ApiFileService = _apiFileService;
                return View(viewModel);
            }

            try
            {
                var sponsor = await _context.Sponsors.FindAsync(id);
                if (sponsor == null)
                {
                    _logger.LogWarning("Sponsor with ID {Id} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Found existing sponsor: {Title}", sponsor.TitleEn);

                // Update properties
                sponsor.TitleUz = viewModel.TitleUz?.Trim() ?? "";
                sponsor.TitleRu = viewModel.TitleRu?.Trim() ?? "";
                sponsor.TitleEn = viewModel.TitleEn?.Trim() ?? "";
                sponsor.Link = viewModel.Link?.Trim() ?? "";
                sponsor.DisplayOrder = viewModel.DisplayOrder;
                sponsor.Status = viewModel.Status;
                sponsor.UpdatedAt = DateTime.UtcNow;

                // Handle photo upload via API
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    _logger.LogInformation("Updating photo via API...");
                    // Delete old photo from API
                    if (!string.IsNullOrEmpty(sponsor.PhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(sponsor.PhotoUrl);
                    }
                    sponsor.PhotoUrl = await _apiFileService.SaveFileAsync(viewModel.Photo, "sponsors");
                    _logger.LogInformation("Photo updated via API: {Url}", sponsor.PhotoUrl);
                }

                _logger.LogInformation("Saving changes...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("=== EDIT SPONSOR COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = "Sponsor updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating sponsor {Id}", id);
                if (!SponsorExists(viewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sponsor {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the sponsor: " + ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // GET: Sponsors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(m => m.Id == id);
            if (sponsor == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(sponsor);
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var sponsor = await _context.Sponsors.FindAsync(id);
                if (sponsor != null)
                {
                    _logger.LogInformation("Deleting associated photo from API...");
                    // Delete associated photo from API
                    if (!string.IsNullOrEmpty(sponsor.PhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(sponsor.PhotoUrl);
                        _logger.LogInformation("Deleted photo from API: {Url}", sponsor.PhotoUrl);
                    }

                    _context.Sponsors.Remove(sponsor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Sponsor deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sponsor");
                TempData["ErrorMessage"] = "An error occurred while deleting the sponsor: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SponsorExists(int id)
        {
            return _context.Sponsors.Any(e => e.Id == id);
        }

        private SponsorViewModel MapToViewModel(Sponsor sponsor)
        {
            return new SponsorViewModel
            {
                Id = sponsor.Id,
                TitleUz = sponsor.TitleUz,
                TitleRu = sponsor.TitleRu,
                TitleEn = sponsor.TitleEn,
                PhotoUrl = sponsor.PhotoUrl,
                Link = sponsor.Link,
                DisplayOrder = sponsor.DisplayOrder,
                Status = sponsor.Status,
                CreatedAt = sponsor.CreatedAt,
                UpdatedAt = sponsor.UpdatedAt
            };
        }
    }
}