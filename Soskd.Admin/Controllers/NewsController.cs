// Soskd.Admin/Controllers/NewsController.cs (Updated)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;
using Soskd.Infrastructure.Services;
using Soskd.Admin.Services; // AAA - Added for ApiFileService
using System.Text.RegularExpressions;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService; // AAA - Keep for backwards compatibility
        private readonly IApiFileService _apiFileService; // AAA - New API file service
        private readonly ILogger<NewsController> _logger;

        public NewsController(
            ApplicationDbContext context,
            IFileService fileService,
            IApiFileService apiFileService, // AAA - Inject API file service
            ILogger<NewsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _apiFileService = apiFileService; // AAA - Store API file service
            _logger = logger;
        }

        // GET: News
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Loading news index page {Page}", page);

                var totalCount = await _context.News.CountAsync();
                var news = await _context.News
                    .OrderByDescending(n => n.PublishedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new NewsItemViewModel
                    {
                        Id = n.Id,
                        TitleUz = n.TitleUz,
                        TitleRu = n.TitleRu,
                        TitleEn = n.TitleEn,
                        SlugUz = n.SlugUz,
                        SlugRu = n.SlugRu,
                        SlugEn = n.SlugEn,
                        SmallPhotoUrl = n.SmallPhotoUrl, // AAA - This will be converted to API URL in view
                        Status = n.Status,
                        PublishedDate = n.PublishedDate,
                        CategoryDisplayEn = n.CategoryEn ?? ""
                    })
                    .ToListAsync();

                var viewModel = new NewsListViewModel
                {
                    News = news,
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
                _logger.LogError(ex, "Error loading news index");
                TempData["ErrorMessage"] = $"Error loading news: {ex.Message}";
                return View(new NewsListViewModel { News = new List<NewsItemViewModel>() });
            }
        }

        // GET: News/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Loading create news page");

            var viewModel = new NewsViewModel
            {
                PublishedDate = DateTime.Now,
                Status = NewsStatus.Published
            };

            ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
            return View(viewModel);
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsViewModel viewModel)
        {
            _logger.LogInformation("=== CREATE NEWS REQUEST STARTED ===");
            _logger.LogInformation("Model State Valid: {IsValid}", ModelState.IsValid);

            // Log all received data
            _logger.LogInformation("Received Data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- DescriptionUz length: {Length}", viewModel.DescriptionUz?.Length ?? 0);
            _logger.LogInformation("- DescriptionRu length: {Length}", viewModel.DescriptionRu?.Length ?? 0);
            _logger.LogInformation("- DescriptionEn length: {Length}", viewModel.DescriptionEn?.Length ?? 0);
            _logger.LogInformation("- Category: {Category}", viewModel.Category?.ToString() ?? "NULL");
            _logger.LogInformation("- Status: {Status}", viewModel.Status);
            _logger.LogInformation("- PublishedDate: {Date} (Kind: {Kind})", viewModel.PublishedDate, viewModel.PublishedDate.Kind);
            _logger.LogInformation("- SmallPhoto: {HasPhoto}", viewModel.SmallPhoto != null ? $"Yes ({viewModel.SmallPhoto.Length} bytes)" : "No");
            _logger.LogInformation("- LargePhoto: {HasPhoto}", viewModel.LargePhoto != null ? $"Yes ({viewModel.LargePhoto.Length} bytes)" : "No");

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

                ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
                return View(viewModel);
            }

            try
            {
                _logger.LogInformation("Creating news entity...");

                // CRITICAL FIX: Convert DateTime to UTC for PostgreSQL
                var publishedDateUtc = viewModel.PublishedDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishedDate, DateTimeKind.Utc)
                    : viewModel.PublishedDate.ToUniversalTime();

                var currentUtc = DateTime.UtcNow;

                _logger.LogInformation("DateTime conversion - Original: {Original} (Kind: {OriginalKind}), UTC: {Utc} (Kind: {UtcKind})",
                    viewModel.PublishedDate, viewModel.PublishedDate.Kind, publishedDateUtc, publishedDateUtc.Kind);

                var news = new News
                {
                    TitleUz = viewModel.TitleUz?.Trim() ?? "",
                    TitleRu = viewModel.TitleRu?.Trim() ?? "",
                    TitleEn = viewModel.TitleEn?.Trim() ?? "",
                    DescriptionUz = viewModel.DescriptionUz ?? "",
                    DescriptionRu = viewModel.DescriptionRu ?? "",
                    DescriptionEn = viewModel.DescriptionEn ?? "",
                    Status = viewModel.Status,
                    PublishedDate = publishedDateUtc, // Use UTC version
                    CreatedAt = currentUtc,          // Use UTC
                    UpdatedAt = currentUtc           // Use UTC
                };

                _logger.LogInformation("Setting category...");
                // Handle category
                if (viewModel.Category.HasValue)
                {
                    var translations = viewModel.Category.Value.GetTranslations();
                    news.CategoryUz = translations.Uz;
                    news.CategoryRu = translations.Ru;
                    news.CategoryEn = translations.En;
                    _logger.LogInformation("Category set: {CategoryEn}", translations.En);
                }

                _logger.LogInformation("AAA - Handling small photo upload via API...");
                // AAA - Handle small photo upload via API (required)
                if (viewModel.SmallPhoto != null && viewModel.SmallPhoto.Length > 0)
                {
                    news.SmallPhotoUrl = await _apiFileService.SaveFileAsync(viewModel.SmallPhoto, "news");
                    _logger.LogInformation("AAA - Small photo saved via API: {Url}", news.SmallPhotoUrl);
                }
                else
                {
                    _logger.LogError("Small photo is required but not provided");
                    ModelState.AddModelError("SmallPhoto", "Small photo is required.");
                    ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
                    return View(viewModel);
                }

                _logger.LogInformation("AAA - Handling large photo upload via API...");
                // AAA - Handle large photo upload via API (optional)
                if (viewModel.LargePhoto != null && viewModel.LargePhoto.Length > 0)
                {
                    news.LargePhotoUrl = await _apiFileService.SaveFileAsync(viewModel.LargePhoto, "news");
                    _logger.LogInformation("AAA - Large photo saved via API: {Url}", news.LargePhotoUrl);
                }

                _logger.LogInformation("Adding news to context...");
                _context.Add(news);

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("News saved with ID: {Id}", news.Id);

                // Set default values for optional fields
                _logger.LogInformation("Setting default optional fields...");
                SetDefaultOptionalFields(news, viewModel);

                _logger.LogInformation("Updating news with default values...");
                _context.Update(news);
                await _context.SaveChangesAsync();

                _logger.LogInformation("=== CREATE NEWS COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = "News created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                ModelState.AddModelError("", $"An error occurred while creating the news: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
            return View(viewModel);
        }

        private void SetDefaultOptionalFields(News news, NewsViewModel viewModel)
        {
            _logger.LogInformation("Setting default optional fields for news ID: {Id}", news.Id);

            // Set slugs - use provided value or generate from title or use ID
            news.SlugUz = !string.IsNullOrWhiteSpace(viewModel.SlugUz) ? viewModel.SlugUz :
                         GenerateSlug(news.TitleUz) ?? news.Id.ToString();
            news.SlugRu = !string.IsNullOrWhiteSpace(viewModel.SlugRu) ? viewModel.SlugRu :
                         GenerateSlug(news.TitleRu) ?? news.Id.ToString();
            news.SlugEn = !string.IsNullOrWhiteSpace(viewModel.SlugEn) ? viewModel.SlugEn :
                         GenerateSlug(news.TitleEn) ?? news.Id.ToString();

            // Set meta titles - use provided value or use title
            news.MetaTitleUz = !string.IsNullOrWhiteSpace(viewModel.MetaTitleUz) ? viewModel.MetaTitleUz : news.TitleUz;
            news.MetaTitleRu = !string.IsNullOrWhiteSpace(viewModel.MetaTitleRu) ? viewModel.MetaTitleRu : news.TitleRu;
            news.MetaTitleEn = !string.IsNullOrWhiteSpace(viewModel.MetaTitleEn) ? viewModel.MetaTitleEn : news.TitleEn;

            // Set meta descriptions - use provided value or use title
            news.MetaDescriptionUz = !string.IsNullOrWhiteSpace(viewModel.MetaDescriptionUz) ? viewModel.MetaDescriptionUz : news.TitleUz;
            news.MetaDescriptionRu = !string.IsNullOrWhiteSpace(viewModel.MetaDescriptionRu) ? viewModel.MetaDescriptionRu : news.TitleRu;
            news.MetaDescriptionEn = !string.IsNullOrWhiteSpace(viewModel.MetaDescriptionEn) ? viewModel.MetaDescriptionEn : news.TitleEn;

            _logger.LogInformation("Generated slugs - Uz: {SlugUz}, Ru: {SlugRu}, En: {SlugEn}",
                news.SlugUz, news.SlugRu, news.SlugEn);
        }

        private string? GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            _logger.LogInformation("Generating advanced slug from: '{Title}'", title);

            // Step 1: Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Step 2: Handle Cyrillic characters (if needed for Uzbek/Russian)
            var cyrillicToLatin = new Dictionary<char, string>
    {
        {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"}, {'ё', "yo"},
        {'ж', "zh"}, {'з', "z"}, {'и', "i"}, {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"},
        {'н', "n"}, {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"},
        {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"},
        {'ъ', ""}, {'ы', "y"}, {'ь', ""}, {'э', "e"}, {'ю', "yu"}, {'я', "ya"},
        // Add Uzbek specific characters if needed
        {'ў', "o"}, {'қ', "q"}, {'ҳ', "h"}, {'ғ', "g"}
    };

            foreach (var pair in cyrillicToLatin)
            {
                slug = slug.Replace(pair.Key.ToString(), pair.Value);
            }

            // Step 3: Normalize spaces
            slug = Regex.Replace(slug, @"\s+", " ");

            // Step 4: Replace spaces with hyphens
            slug = slug.Replace(" ", "-");

            // Step 5: Remove special characters except hyphens and basic Latin letters/numbers
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Step 6: Remove multiple consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Step 7: Trim hyphens
            slug = slug.Trim('-');

            // Step 8: Fallback
            if (string.IsNullOrWhiteSpace(slug))
                slug = "untitled";

            _logger.LogInformation("Final advanced slug: '{Slug}'", slug);
            return slug;
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(news);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(news);
            ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: News/Edit/5 - ROBUST VERSION with EF tracking fixes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsViewModel viewModel)
        {
            _logger.LogInformation("=== EDIT NEWS REQUEST STARTED ===");
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

                ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
                ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
                return View(viewModel);
            }

            try
            {
                // Fetch the entity and ensure it's tracked
                var news = await _context.News.FindAsync(id);
                if (news == null)
                {
                    _logger.LogWarning("News with ID {Id} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Found existing news: {Title}", news.TitleEn);
                _logger.LogInformation("Entity state before update: {State}", _context.Entry(news).State);

                // Log original values for comparison
                _logger.LogInformation("BEFORE UPDATE:");
                _logger.LogInformation("- TitleEn: '{TitleEn}'", news.TitleEn);
                _logger.LogInformation("- Status: {Status}", news.Status);
                _logger.LogInformation("- PublishedDate: {Date}", news.PublishedDate);

                // CRITICAL FIX: Convert DateTime to UTC for PostgreSQL
                var publishedDateUtc = viewModel.PublishedDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishedDate, DateTimeKind.Utc)
                    : viewModel.PublishedDate.ToUniversalTime();

                _logger.LogInformation("DateTime conversion - Original: {Original} (Kind: {OriginalKind}), UTC: {Utc} (Kind: {UtcKind})",
                    viewModel.PublishedDate, viewModel.PublishedDate.Kind, publishedDateUtc, publishedDateUtc.Kind);

                // Update properties one by one and log changes
                if (news.TitleUz != viewModel.TitleUz?.Trim())
                {
                    _logger.LogInformation("Updating TitleUz: '{Old}' -> '{New}'", news.TitleUz, viewModel.TitleUz?.Trim());
                    news.TitleUz = viewModel.TitleUz?.Trim() ?? "";
                }

                if (news.TitleRu != viewModel.TitleRu?.Trim())
                {
                    _logger.LogInformation("Updating TitleRu: '{Old}' -> '{New}'", news.TitleRu, viewModel.TitleRu?.Trim());
                    news.TitleRu = viewModel.TitleRu?.Trim() ?? "";
                }

                if (news.TitleEn != viewModel.TitleEn?.Trim())
                {
                    _logger.LogInformation("Updating TitleEn: '{Old}' -> '{New}'", news.TitleEn, viewModel.TitleEn?.Trim());
                    news.TitleEn = viewModel.TitleEn?.Trim() ?? "";
                }

                if (news.DescriptionUz != viewModel.DescriptionUz)
                {
                    _logger.LogInformation("Updating DescriptionUz length: {OldLength} -> {NewLength}",
                        news.DescriptionUz?.Length ?? 0, viewModel.DescriptionUz?.Length ?? 0);
                    news.DescriptionUz = viewModel.DescriptionUz ?? "";
                }

                if (news.DescriptionRu != viewModel.DescriptionRu)
                {
                    _logger.LogInformation("Updating DescriptionRu length: {OldLength} -> {NewLength}",
                        news.DescriptionRu?.Length ?? 0, viewModel.DescriptionRu?.Length ?? 0);
                    news.DescriptionRu = viewModel.DescriptionRu ?? "";
                }

                if (news.DescriptionEn != viewModel.DescriptionEn)
                {
                    _logger.LogInformation("Updating DescriptionEn length: {OldLength} -> {NewLength}",
                        news.DescriptionEn?.Length ?? 0, viewModel.DescriptionEn?.Length ?? 0);
                    news.DescriptionEn = viewModel.DescriptionEn ?? "";
                }

                if (news.Status != viewModel.Status)
                {
                    _logger.LogInformation("Updating Status: {Old} -> {New}", news.Status, viewModel.Status);
                    news.Status = viewModel.Status;
                }

                if (news.PublishedDate != publishedDateUtc)
                {
                    _logger.LogInformation("Updating PublishedDate: {Old} -> {New}", news.PublishedDate, publishedDateUtc);
                    news.PublishedDate = publishedDateUtc;
                }

                // Always update the UpdatedAt timestamp
                news.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Setting UpdatedAt to: {UpdatedAt}", news.UpdatedAt);

                // Handle category
                if (viewModel.Category.HasValue)
                {
                    var translations = viewModel.Category.Value.GetTranslations();
                    if (news.CategoryEn != translations.En)
                    {
                        _logger.LogInformation("Updating Category: '{Old}' -> '{New}'", news.CategoryEn, translations.En);
                        news.CategoryUz = translations.Uz;
                        news.CategoryRu = translations.Ru;
                        news.CategoryEn = translations.En;
                    }
                }
                else if (!string.IsNullOrEmpty(news.CategoryEn))
                {
                    _logger.LogInformation("Clearing category");
                    news.CategoryUz = null;
                    news.CategoryRu = null;
                    news.CategoryEn = null;
                }

                // Set optional fields with defaults if empty
                SetDefaultOptionalFields(news, viewModel);

                // AAA - Handle small photo upload via API
                if (viewModel.SmallPhoto != null && viewModel.SmallPhoto.Length > 0)
                {
                    _logger.LogInformation("AAA - Updating small photo via API...");
                    // Delete old photo from API
                    if (!string.IsNullOrEmpty(news.SmallPhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(news.SmallPhotoUrl);
                    }
                    news.SmallPhotoUrl = await _apiFileService.SaveFileAsync(viewModel.SmallPhoto, "news");
                    _logger.LogInformation("AAA - Small photo updated via API: {Url}", news.SmallPhotoUrl);
                }

                // AAA - Handle large photo upload via API
                if (viewModel.LargePhoto != null && viewModel.LargePhoto.Length > 0)
                {
                    _logger.LogInformation("AAA - Updating large photo via API...");
                    // Delete old photo from API
                    if (!string.IsNullOrEmpty(news.LargePhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(news.LargePhotoUrl);
                    }
                    news.LargePhotoUrl = await _apiFileService.SaveFileAsync(viewModel.LargePhoto, "news");
                    _logger.LogInformation("AAA - Large photo updated via API: {Url}", news.LargePhotoUrl);
                }

                // Check entity state before saving
                _logger.LogInformation("Entity state before save: {State}", _context.Entry(news).State);

                // Log all modified properties
                var modifiedProperties = _context.Entry(news).Properties
                    .Where(p => p.IsModified)
                    .Select(p => p.Metadata.Name)
                    .ToList();

                if (modifiedProperties.Any())
                {
                    _logger.LogInformation("Modified properties: {Properties}", string.Join(", ", modifiedProperties));
                }
                else
                {
                    _logger.LogWarning("No properties were marked as modified!");
                    // Force update if no changes detected
                    _context.Entry(news).State = EntityState.Modified;
                }

                _logger.LogInformation("Calling SaveChangesAsync...");
                var changeCount = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed. Changes saved: {ChangeCount}", changeCount);

                if (changeCount == 0)
                {
                    _logger.LogWarning("SaveChangesAsync returned 0 - no changes were saved to database!");
                }

                // Verify the update by reloading from database
                await _context.Entry(news).ReloadAsync();
                _logger.LogInformation("AFTER RELOAD FROM DB:");
                _logger.LogInformation("- TitleEn: '{TitleEn}'", news.TitleEn);
                _logger.LogInformation("- Status: {Status}", news.Status);
                _logger.LogInformation("- UpdatedAt: {UpdatedAt}", news.UpdatedAt);

                _logger.LogInformation("=== EDIT NEWS COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = $"News updated successfully! Changes saved: {changeCount}";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating news {Id}", id);
                if (!NewsExists(viewModel.Id))
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
                _logger.LogError(ex, "Error updating news {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the news: " + ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.Categories = NewsCategoryExtensions.GetAllCategories();
            ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
            return View(viewModel);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(news);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var news = await _context.News.FindAsync(id);
                if (news != null)
                {
                    _logger.LogInformation("AAA - Deleting associated files from API...");
                    // AAA - Delete associated files from API
                    if (!string.IsNullOrEmpty(news.SmallPhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(news.SmallPhotoUrl);
                        _logger.LogInformation("AAA - Deleted small photo from API: {Url}", news.SmallPhotoUrl);
                    }
                    if (!string.IsNullOrEmpty(news.LargePhotoUrl))
                    {
                        await _apiFileService.DeleteFileAsync(news.LargePhotoUrl);
                        _logger.LogInformation("AAA - Deleted large photo from API: {Url}", news.LargePhotoUrl);
                    }

                    _context.News.Remove(news);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "News deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error deleting news");
                TempData["ErrorMessage"] = "An error occurred while deleting the news: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        private NewsViewModel MapToViewModel(News news)
        {
            NewsCategory? category = null;
            if (!string.IsNullOrEmpty(news.CategoryEn))
            {
                category = news.CategoryEn switch
                {
                    "Education" => NewsCategory.Education,
                    "Events" => NewsCategory.Events,
                    "Memorandums" => NewsCategory.Memorandums,
                    _ => null
                };
            }

            return new NewsViewModel
            {
                Id = news.Id,
                TitleUz = news.TitleUz,
                TitleRu = news.TitleRu,
                TitleEn = news.TitleEn,
                DescriptionUz = news.DescriptionUz,
                DescriptionRu = news.DescriptionRu,
                DescriptionEn = news.DescriptionEn,
                SlugUz = news.SlugUz,
                SlugRu = news.SlugRu,
                SlugEn = news.SlugEn,
                MetaTitleUz = news.MetaTitleUz,
                MetaTitleRu = news.MetaTitleRu,
                MetaTitleEn = news.MetaTitleEn,
                MetaDescriptionUz = news.MetaDescriptionUz,
                MetaDescriptionRu = news.MetaDescriptionRu,
                MetaDescriptionEn = news.MetaDescriptionEn,
                Category = category,
                Status = news.Status,
                SmallPhotoUrl = news.SmallPhotoUrl, // AAA - This will be converted to API URL in views
                LargePhotoUrl = news.LargePhotoUrl, // AAA - This will be converted to API URL in views
                PublishedDate = news.PublishedDate,
                CreatedAt = news.CreatedAt,
                UpdatedAt = news.UpdatedAt
            };
        }

        // GET: News/TestCreate
        public IActionResult TestCreate()
        {
            var viewModel = new NewsViewModel
            {
                PublishedDate = DateTime.Now,
                Status = NewsStatus.Published
            };

            return View(viewModel);
        }

        // POST: News/TestCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestCreate(NewsViewModel viewModel, string FakePhotoUrl)
        {
            _logger.LogInformation("=== TEST CREATE NEWS STARTED ===");
            _logger.LogInformation("Received test data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- DescriptionUz: '{DescriptionUz}'", viewModel.DescriptionUz ?? "NULL");
            _logger.LogInformation("- DescriptionRu: '{DescriptionRu}'", viewModel.DescriptionRu ?? "NULL");
            _logger.LogInformation("- DescriptionEn: '{DescriptionEn}'", viewModel.DescriptionEn ?? "NULL");
            _logger.LogInformation("- Status: {Status}", viewModel.Status);
            _logger.LogInformation("- PublishedDate: {Date}", viewModel.PublishedDate);
            _logger.LogInformation("- FakePhotoUrl: '{Url}'", FakePhotoUrl ?? "NULL");

            // Manual validation for test
            if (string.IsNullOrWhiteSpace(viewModel.TitleUz))
                ModelState.AddModelError("TitleUz", "Title UZ required for test");
            if (string.IsNullOrWhiteSpace(viewModel.TitleRu))
                ModelState.AddModelError("TitleRu", "Title RU required for test");
            if (string.IsNullOrWhiteSpace(viewModel.TitleEn))
                ModelState.AddModelError("TitleEn", "Title EN required for test");
            if (string.IsNullOrWhiteSpace(viewModel.DescriptionUz))
                ModelState.AddModelError("DescriptionUz", "Description UZ required for test");
            if (string.IsNullOrWhiteSpace(viewModel.DescriptionRu))
                ModelState.AddModelError("DescriptionRu", "Description RU required for test");
            if (string.IsNullOrWhiteSpace(viewModel.DescriptionEn))
                ModelState.AddModelError("DescriptionEn", "Description EN required for test");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("TEST: ModelState validation failed");
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
                _logger.LogInformation("TEST: Creating minimal news entity...");

                var news = new News
                {
                    TitleUz = viewModel.TitleUz.Trim(),
                    TitleRu = viewModel.TitleRu.Trim(),
                    TitleEn = viewModel.TitleEn.Trim(),
                    DescriptionUz = viewModel.DescriptionUz,
                    DescriptionRu = viewModel.DescriptionRu,
                    DescriptionEn = viewModel.DescriptionEn,
                    SmallPhotoUrl = FakePhotoUrl ?? "/uploads/news/test.jpg", // Use fake URL
                    Status = viewModel.Status,
                    PublishedDate = viewModel.PublishedDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle category if provided
                if (viewModel.Category.HasValue)
                {
                    var translations = viewModel.Category.Value.GetTranslations();
                    news.CategoryUz = translations.Uz;
                    news.CategoryRu = translations.Ru;
                    news.CategoryEn = translations.En;
                    _logger.LogInformation("TEST: Category set to {Category}", translations.En);
                }

                _logger.LogInformation("TEST: Adding to database...");
                _context.Add(news);

                _logger.LogInformation("TEST: Saving changes...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("TEST: News created with ID {Id}", news.Id);

                // Try to set optional fields if database has them
                try
                {
                    _logger.LogInformation("TEST: Setting optional fields...");
                    news.SlugUz = $"test-slug-uz-{news.Id}";
                    news.SlugRu = $"test-slug-ru-{news.Id}";
                    news.SlugEn = $"test-slug-en-{news.Id}";
                    news.MetaTitleUz = news.TitleUz;
                    news.MetaTitleRu = news.TitleRu;
                    news.MetaTitleEn = news.TitleEn;
                    news.MetaDescriptionUz = news.TitleUz;
                    news.MetaDescriptionRu = news.TitleRu;
                    news.MetaDescriptionEn = news.TitleEn;

                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("TEST: Optional fields set successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("TEST: Could not set optional fields (probably migration not applied): {Error}", ex.Message);
                    // Continue anyway - basic news was created
                }

                _logger.LogInformation("=== TEST CREATE COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = "🧪 TEST: News created successfully! ID: " + news.Id;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TEST: Error creating news");
                ModelState.AddModelError("", $"TEST ERROR: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "TEST: Inner exception");
                    ModelState.AddModelError("", $"Inner: {ex.InnerException.Message}");
                }
            }

            return View(viewModel);
        }
    }
}