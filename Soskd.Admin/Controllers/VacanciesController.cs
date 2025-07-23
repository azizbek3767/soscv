using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class VacanciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VacanciesController> _logger;

        public VacanciesController(ApplicationDbContext context, ILogger<VacanciesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Vacancies
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Loading vacancies index page {Page}", page);

                var totalCount = await _context.Vacancies.CountAsync();
                var vacancies = await _context.Vacancies
                    .OrderByDescending(v => v.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(v => new VacancyItemViewModel
                    {
                        Id = v.Id,
                        TitleUz = v.TitleUz,
                        TitleRu = v.TitleRu,
                        TitleEn = v.TitleEn,
                        Status = v.Status,
                        PublishedDate = v.PublishedDate,
                        Deadline = v.Deadline,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync();

                var viewModel = new VacancyListViewModel
                {
                    Vacancies = vacancies,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vacancies index");
                TempData["ErrorMessage"] = $"Error loading vacancies: {ex.Message}";
                return View(new VacancyListViewModel { Vacancies = new List<VacancyItemViewModel>() });
            }
        }

        // GET: Vacancies/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Loading create vacancy page");

            var viewModel = new VacancyViewModel
            {
                Status = VacancyStatus.Published,
                PublishedDate = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30) // Default deadline 30 days from now
            };

            return View(viewModel);
        }

        // POST: Vacancies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacancyViewModel viewModel)
        {
            _logger.LogInformation("=== CREATE VACANCY REQUEST STARTED ===");
            _logger.LogInformation("Model State Valid: {IsValid}", ModelState.IsValid);

            // Log all received data
            _logger.LogInformation("Received Data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- DescriptionUz length: {Length}", viewModel.DescriptionUz?.Length ?? 0);
            _logger.LogInformation("- DescriptionRu length: {Length}", viewModel.DescriptionRu?.Length ?? 0);
            _logger.LogInformation("- DescriptionEn length: {Length}", viewModel.DescriptionEn?.Length ?? 0);
            _logger.LogInformation("- PublishedDate: {Date}", viewModel.PublishedDate?.ToString() ?? "NULL");
            _logger.LogInformation("- Deadline: {Date}", viewModel.Deadline?.ToString() ?? "NULL");

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
                _logger.LogInformation("Creating vacancy entity...");

                // Convert DateTime to UTC for PostgreSQL
                var publishedDateUtc = viewModel.PublishedDate.HasValue
                    ? (viewModel.PublishedDate.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(viewModel.PublishedDate.Value, DateTimeKind.Utc)
                        : viewModel.PublishedDate.Value.ToUniversalTime())
                    : (DateTime?)null;

                var deadlineUtc = viewModel.Deadline.HasValue
                    ? (viewModel.Deadline.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(viewModel.Deadline.Value, DateTimeKind.Utc)
                        : viewModel.Deadline.Value.ToUniversalTime())
                    : (DateTime?)null;

                var currentUtc = DateTime.UtcNow;

                var vacancy = new Vacancy
                {
                    TitleUz = viewModel.TitleUz?.Trim() ?? "",
                    TitleRu = viewModel.TitleRu?.Trim() ?? "",
                    TitleEn = viewModel.TitleEn?.Trim() ?? "",
                    DescriptionUz = viewModel.DescriptionUz ?? "",
                    DescriptionRu = viewModel.DescriptionRu ?? "",
                    DescriptionEn = viewModel.DescriptionEn ?? "",
                    Status = viewModel.Status,
                    PublishedDate = publishedDateUtc,
                    Deadline = deadlineUtc,
                    CreatedAt = currentUtc,
                    UpdatedAt = currentUtc
                };

                _logger.LogInformation("Adding vacancy to context...");
                _context.Add(vacancy);

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vacancy saved with ID: {Id}", vacancy.Id);
                _logger.LogInformation("=== CREATE VACANCY COMPLETED SUCCESSFULLY ===");

                TempData["SuccessMessage"] = "Vacancy created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vacancy");
                ModelState.AddModelError("", $"An error occurred while creating the vacancy: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: Vacancies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(vacancy);
            return View(viewModel);
        }

        // GET: Vacancies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(vacancy);
            return View(viewModel);
        }

        // POST: Vacancies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VacancyViewModel viewModel)
        {
            _logger.LogInformation("=== EDIT VACANCY REQUEST STARTED ===");
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

                return View(viewModel);
            }

            try
            {
                // Fetch the entity and ensure it's tracked
                var vacancy = await _context.Vacancies.FindAsync(id);
                if (vacancy == null)
                {
                    _logger.LogWarning("Vacancy with ID {Id} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Found existing vacancy: {Title}", vacancy.TitleEn);

                // Convert DateTime to UTC for PostgreSQL
                var publishedDateUtc = viewModel.PublishedDate.HasValue
                    ? (viewModel.PublishedDate.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(viewModel.PublishedDate.Value, DateTimeKind.Utc)
                        : viewModel.PublishedDate.Value.ToUniversalTime())
                    : (DateTime?)null;

                var deadlineUtc = viewModel.Deadline.HasValue
                    ? (viewModel.Deadline.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(viewModel.Deadline.Value, DateTimeKind.Utc)
                        : viewModel.Deadline.Value.ToUniversalTime())
                    : (DateTime?)null;

                // Update properties
                vacancy.TitleUz = viewModel.TitleUz?.Trim() ?? "";
                vacancy.TitleRu = viewModel.TitleRu?.Trim() ?? "";
                vacancy.TitleEn = viewModel.TitleEn?.Trim() ?? "";
                vacancy.DescriptionUz = viewModel.DescriptionUz ?? "";
                vacancy.DescriptionRu = viewModel.DescriptionRu ?? "";
                vacancy.DescriptionEn = viewModel.DescriptionEn ?? "";
                vacancy.Status = viewModel.Status;
                vacancy.PublishedDate = publishedDateUtc;
                vacancy.Deadline = deadlineUtc;
                vacancy.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Calling SaveChangesAsync...");
                var changeCount = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed. Changes saved: {ChangeCount}", changeCount);

                _logger.LogInformation("=== EDIT VACANCY COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = $"Vacancy updated successfully! Changes saved: {changeCount}";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating vacancy {Id}", id);
                if (!VacancyExists(viewModel.Id))
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
                _logger.LogError(ex, "Error updating vacancy {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the vacancy: " + ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            return View(viewModel);
        }

        // GET: Vacancies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(vacancy);
            return View(viewModel);
        }

        // POST: Vacancies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vacancy = await _context.Vacancies.FindAsync(id);
                if (vacancy != null)
                {
                    _context.Vacancies.Remove(vacancy);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vacancy deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the vacancy: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VacancyExists(int id)
        {
            return _context.Vacancies.Any(e => e.Id == id);
        }

        private VacancyViewModel MapToViewModel(Vacancy vacancy)
        {
            return new VacancyViewModel
            {
                Id = vacancy.Id,
                TitleUz = vacancy.TitleUz,
                TitleRu = vacancy.TitleRu,
                TitleEn = vacancy.TitleEn,
                DescriptionUz = vacancy.DescriptionUz,
                DescriptionRu = vacancy.DescriptionRu,
                DescriptionEn = vacancy.DescriptionEn,
                Status = vacancy.Status,
                PublishedDate = vacancy.PublishedDate,
                Deadline = vacancy.Deadline,
                CreatedAt = vacancy.CreatedAt,
                UpdatedAt = vacancy.UpdatedAt
            };
        }
    }
}