// Soskd.Admin/Controllers/MediaAboutUsController.cs (Updated)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;
using Soskd.Admin.Services; // AAA - Added for ApiFileService

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MediaAboutUsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService; // AAA - Keep for backwards compatibility
        private readonly IApiFileService _apiFileService; // AAA - New API file service
        private readonly ILogger<MediaAboutUsController> _logger;

        public MediaAboutUsController(
            ApplicationDbContext context,
            IFileService fileService,
            IApiFileService apiFileService, // AAA - Inject API file service
            ILogger<MediaAboutUsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _apiFileService = apiFileService; // AAA - Store API file service
            _logger = logger;
        }

        // GET: MediaAboutUs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Loading media about us index page {Page}", page);

                var totalCount = await _context.MediaAboutUs.CountAsync();
                var mediaItems = await _context.MediaAboutUs
                    .OrderByDescending(m => m.PublishDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MediaAboutUsItemViewModel
                    {
                        Id = m.Id,
                        TitleUz = m.TitleUz,
                        TitleRu = m.TitleRu,
                        TitleEn = m.TitleEn,
                        PhotoUrl = m.PhotoUrl, // AAA - This will be converted to API URL in view
                        SourceLink = m.SourceLink,
                        Status = m.Status,
                        PublishDate = m.PublishDate,
                        CreatedAt = m.CreatedAt
                    })
                    .ToListAsync();

                var viewModel = new MediaAboutUsListViewModel
                {
                    MediaItems = mediaItems,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                // AAA - Pass API file service to view for URL conversion
                ViewBag.ApiFileService = _apiFileService;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading media about us index");
                TempData["ErrorMessage"] = $"Error loading media: {ex.Message}";
                return View(new MediaAboutUsListViewModel { MediaItems = new List<MediaAboutUsItemViewModel>() });
            }
        }

        // GET: MediaAboutUs/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Loading create media about us page");

            var viewModel = new MediaAboutUsViewModel
            {
                PublishDate = DateTime.Now,
                Status = MediaStatus.Published
            };

            return View(viewModel);
        }

        // POST: MediaAboutUs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MediaAboutUsViewModel viewModel)
        {
            _logger.LogInformation("=== CREATE MEDIA ABOUT US REQUEST STARTED ===");
            _logger.LogInformation("Model State Valid: {IsValid}", ModelState.IsValid);

            // Log all received data
            _logger.LogInformation("Received Data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- DescriptionUz length: {Length}", viewModel.DescriptionUz?.Length ?? 0);
            _logger.LogInformation("- DescriptionRu length: {Length}", viewModel.DescriptionRu?.Length ?? 0);
            _logger.LogInformation("- DescriptionEn length: {Length}", viewModel.DescriptionEn?.Length ?? 0);
            _logger.LogInformation("- SourceLink: '{SourceLink}'", viewModel.SourceLink ?? "NULL");
            _logger.LogInformation("- Status: {Status}", viewModel.Status);
            _logger.LogInformation("- PublishDate: {Date} (Kind: {Kind})", viewModel.PublishDate, viewModel.PublishDate.Kind);
            _logger.LogInformation("- Photo: {HasPhoto}", viewModel.Photo != null ? $"Yes ({viewModel.Photo.Length} bytes)" : "No");

            // Log ModelState errors
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
                _logger.LogInformation("Creating media about us entity...");

                // Convert DateTime to UTC for PostgreSQL
                var publishDateUtc = viewModel.PublishDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishDate, DateTimeKind.Utc)
                    : viewModel.PublishDate.ToUniversalTime();

                var currentUtc = DateTime.UtcNow;

                _logger.LogInformation("DateTime conversion - Original: {Original} (Kind: {OriginalKind}), UTC: {Utc} (Kind: {UtcKind})",
                    viewModel.PublishDate, viewModel.PublishDate.Kind, publishDateUtc, publishDateUtc.Kind);

                var mediaAboutUs = new MediaAboutUs
                {
                    TitleUz = viewModel.TitleUz?.Trim() ?? "",
                    TitleRu = viewModel.TitleRu?.Trim() ?? "",
                    TitleEn = viewModel.TitleEn?.Trim() ?? "",
                    DescriptionUz = viewModel.DescriptionUz ?? "",
                    DescriptionRu = viewModel.DescriptionRu ?? "",
                    DescriptionEn = viewModel.DescriptionEn ?? "",
                    SourceLink = viewModel.SourceLink?.Trim() ?? "",
                    Status = viewModel.Status,
                    PublishDate = publishDateUtc,
                    CreatedAt = currentUtc,
                    UpdatedAt = currentUtc
                };

                _logger.LogInformation("AAA - Handling photo upload via API...");
                // AAA - Handle photo upload via API (required)
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    mediaAboutUs.PhotoUrl = await _apiFileService.SaveFileAsync(viewModel.Photo, "media");
                    _logger.LogInformation("AAA - Photo saved via API: {Url}", mediaAboutUs.PhotoUrl);
                }
                else
                {
                    _logger.LogError("Photo is required but not provided");
                    ModelState.AddModelError("Photo", "Photo is required.");
                    return View(viewModel);
                }

                _logger.LogInformation("Adding media about us to context...");
                _context.Add(mediaAboutUs);

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Media about us saved with ID: {Id}", mediaAboutUs.Id);
                _logger.LogInformation("=== CREATE MEDIA ABOUT US COMPLETED SUCCESSFULLY ===");

                TempData["SuccessMessage"] = "Media item created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating media about us");
                ModelState.AddModelError("", $"An error occurred while creating the media item: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: MediaAboutUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaAboutUs = await _context.MediaAboutUs.FirstOrDefaultAsync(m => m.Id == id);
            if (mediaAboutUs == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(mediaAboutUs);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // GET: MediaAboutUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaAboutUs = await _context.MediaAboutUs.FindAsync(id);
            if (mediaAboutUs == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(mediaAboutUs);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: MediaAboutUs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MediaAboutUsViewModel viewModel)
        {
            _logger.LogInformation("=== EDIT MEDIA ABOUT US REQUEST STARTED ===");
            _logger.LogInformation("ID: {Id}, Model State Valid: {IsValid}", id, ModelState.IsValid);

            if (id != viewModel.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} != Model ID {ModelId}", id, viewModel.Id);
                return NotFound();
            }

            // Log ModelState errors
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

                ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
                return View(viewModel);
            }

            try
            {
                // Fetch the entity and ensure it's tracked
                var mediaAboutUs = await _context.MediaAboutUs.FindAsync(id);
                if (mediaAboutUs == null)
                {
                    _logger.LogWarning("Media about us with ID {Id} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Found existing media about us: {Title}", mediaAboutUs.TitleEn);

                // Convert DateTime to UTC for PostgreSQL
                var publishDateUtc = viewModel.PublishDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishDate, DateTimeKind.Utc)
                    : viewModel.PublishDate.ToUniversalTime();

                // Update properties
                mediaAboutUs.TitleUz = viewModel.TitleUz?.Trim() ?? "";
                mediaAboutUs.TitleRu = viewModel.TitleRu?.Trim() ?? "";
                mediaAboutUs.TitleEn = viewModel.TitleEn?.Trim() ?? "";
                mediaAboutUs.DescriptionUz = viewModel.DescriptionUz ?? "";
                mediaAboutUs.DescriptionRu = viewModel.DescriptionRu ?? "";
                mediaAboutUs.DescriptionEn = viewModel.DescriptionEn ?? "";
                mediaAboutUs.SourceLink = viewModel.SourceLink?.Trim() ?? "";
                mediaAboutUs.Status = viewModel.Status;
                mediaAboutUs.PublishDate = publishDateUtc;
                mediaAboutUs.UpdatedAt = DateTime.UtcNow;

                // AAA - Handle photo upload via API
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    _logger.LogInformation("AAA - Updating photo via API...");
                    // AAA - Delete old photo from API
                    if (!string.IsNullOrEmpty(mediaAboutUs.PhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(mediaAboutUs.PhotoUrl);
                        _logger.LogInformation("AAA - Deleted old photo from API: {Url}", mediaAboutUs.PhotoUrl);
                    }
                    mediaAboutUs.PhotoUrl = await _apiFileService.SaveFileAsync(viewModel.Photo, "media");
                    _logger.LogInformation("AAA - Photo updated via API: {Url}", mediaAboutUs.PhotoUrl);
                }

                _logger.LogInformation("Calling SaveChangesAsync...");
                var changeCount = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed. Changes saved: {ChangeCount}", changeCount);

                _logger.LogInformation("=== EDIT MEDIA ABOUT US COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = $"Media item updated successfully! Changes saved: {changeCount}";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating media about us {Id}", id);
                if (!MediaAboutUsExists(viewModel.Id))
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
                _logger.LogError(ex, "Error updating media about us {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the media item: " + ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
            return View(viewModel);
        }

        // GET: MediaAboutUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaAboutUs = await _context.MediaAboutUs.FirstOrDefaultAsync(m => m.Id == id);
            if (mediaAboutUs == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(mediaAboutUs);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: MediaAboutUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var mediaAboutUs = await _context.MediaAboutUs.FindAsync(id);
                if (mediaAboutUs != null)
                {
                    _logger.LogInformation("AAA - Deleting associated photo from API...");
                    // AAA - Delete associated photo from API
                    if (!string.IsNullOrEmpty(mediaAboutUs.PhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(mediaAboutUs.PhotoUrl);
                        _logger.LogInformation("AAA - Deleted photo from API: {Url}", mediaAboutUs.PhotoUrl);
                    }

                    _context.MediaAboutUs.Remove(mediaAboutUs);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Media item deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error deleting media about us");
                TempData["ErrorMessage"] = "An error occurred while deleting the media item: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MediaAboutUsExists(int id)
        {
            return _context.MediaAboutUs.Any(e => e.Id == id);
        }

        private MediaAboutUsViewModel MapToViewModel(MediaAboutUs mediaAboutUs)
        {
            return new MediaAboutUsViewModel
            {
                Id = mediaAboutUs.Id,
                TitleUz = mediaAboutUs.TitleUz,
                TitleRu = mediaAboutUs.TitleRu,
                TitleEn = mediaAboutUs.TitleEn,
                DescriptionUz = mediaAboutUs.DescriptionUz,
                DescriptionRu = mediaAboutUs.DescriptionRu,
                DescriptionEn = mediaAboutUs.DescriptionEn,
                PhotoUrl = mediaAboutUs.PhotoUrl, // AAA - This will be converted to API URL in views
                SourceLink = mediaAboutUs.SourceLink,
                Status = mediaAboutUs.Status,
                PublishDate = mediaAboutUs.PublishDate,
                CreatedAt = mediaAboutUs.CreatedAt,
                UpdatedAt = mediaAboutUs.UpdatedAt
            };
        }
    }
}