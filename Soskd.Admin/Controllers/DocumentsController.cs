// Soskd.Admin/Controllers/DocumentsController.cs (Cleaned)
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
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService; // AAA - Keep for backwards compatibility
        private readonly IApiFileService _apiFileService; // AAA - New API file service
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            ApplicationDbContext context,
            IFileService fileService,
            IApiFileService apiFileService, // AAA - Inject API file service
            ILogger<DocumentsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _apiFileService = apiFileService; // AAA - Store API file service
            _logger = logger;
        }

        // GET: Documents
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Loading documents index page {Page}", page);

                var totalCount = await _context.Documents.CountAsync();
                var documents = await _context.Documents
                    .OrderByDescending(d => d.PublishDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DocumentItemViewModel
                    {
                        Id = d.Id,
                        TitleUz = d.TitleUz,
                        TitleRu = d.TitleRu,
                        TitleEn = d.TitleEn,
                        DescriptionUz = d.DescriptionUz,
                        DescriptionRu = d.DescriptionRu,
                        DescriptionEn = d.DescriptionEn,
                        FileUrl = d.FileUrl, // AAA - This will be converted to API URL in view
                        FileName = d.FileName,
                        FileSizeFormatted = d.FileSizeFormatted,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Added multilingual file properties
                        FileUrlUz = d.FileUrlUz,
                        FileNameUz = d.FileNameUz,
                        FileSizeFormattedUz = d.FileSizeFormattedUz,
                        FileUrlEn = d.FileUrlEn,
                        FileNameEn = d.FileNameEn,
                        FileSizeFormattedEn = d.FileSizeFormattedEn,
                        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of multilingual file properties
                        Status = d.Status,
                        Category = d.Category,
                        PublishDate = d.PublishDate,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                var viewModel = new DocumentListViewModel
                {
                    Documents = documents,
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
                _logger.LogError(ex, "Error loading documents index");
                TempData["ErrorMessage"] = $"Error loading documents: {ex.Message}";
                return View(new DocumentListViewModel { Documents = new List<DocumentItemViewModel>() });
            }
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Loading create document page");

            var viewModel = new DocumentViewModel
            {
                PublishDate = DateTime.Now,
                Status = DocumentStatus.Published,
                Category = DocumentCategory.Documents
            };

            ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
            return View(viewModel);
        }

        // POST: Documents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel viewModel)
        {
            _logger.LogInformation("=== CREATE DOCUMENT REQUEST STARTED ===");
            _logger.LogInformation("Model State Valid: {IsValid}", ModelState.IsValid);

            // Log all received data
            _logger.LogInformation("Received Data:");
            _logger.LogInformation("- TitleUz: '{TitleUz}'", viewModel.TitleUz ?? "NULL");
            _logger.LogInformation("- TitleRu: '{TitleRu}'", viewModel.TitleRu ?? "NULL");
            _logger.LogInformation("- TitleEn: '{TitleEn}'", viewModel.TitleEn ?? "NULL");
            _logger.LogInformation("- Category: {Category}", viewModel.Category);
            _logger.LogInformation("- Status: {Status}", viewModel.Status);
            _logger.LogInformation("- PublishDate: {Date}", viewModel.PublishDate);
            // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Updated logging for optional documents
            _logger.LogInformation("- DocumentUz: {HasDocumentUz}", viewModel.DocumentUz != null ? $"Yes ({viewModel.DocumentUz.Length} bytes)" : "No (Optional)");
            _logger.LogInformation("- Document (RU): {HasDocument}", viewModel.Document != null ? $"Yes ({viewModel.Document.Length} bytes)" : "No (Optional)");
            _logger.LogInformation("- DocumentEn: {HasDocumentEn}", viewModel.DocumentEn != null ? $"Yes ({viewModel.DocumentEn.Length} bytes)" : "No (Optional)");
            // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional document logging

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

                ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                return View(viewModel);
            }

            try
            {
                _logger.LogInformation("Creating document entity...");

                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Updated validation to make files optional
                // Validate PDF files only if provided (optional validation)
                if (viewModel.DocumentUz != null && viewModel.DocumentUz.Length > 0)
                {
                    var fileExtensionUz = Path.GetExtension(viewModel.DocumentUz.FileName).ToLowerInvariant();
                    if (fileExtensionUz != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for Uzbek document. Received: {Extension}", fileExtensionUz);
                        ModelState.AddModelError("DocumentUz", "Only PDF files are allowed for Uzbek document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                }

                if (viewModel.Document != null && viewModel.Document.Length > 0)
                {
                    var fileExtensionRu = Path.GetExtension(viewModel.Document.FileName).ToLowerInvariant();
                    if (fileExtensionRu != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for Russian document. Received: {Extension}", fileExtensionRu);
                        ModelState.AddModelError("Document", "Only PDF files are allowed for Russian document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                }

                if (viewModel.DocumentEn != null && viewModel.DocumentEn.Length > 0)
                {
                    var fileExtensionEn = Path.GetExtension(viewModel.DocumentEn.FileName).ToLowerInvariant();
                    if (fileExtensionEn != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for English document. Received: {Extension}", fileExtensionEn);
                        ModelState.AddModelError("DocumentEn", "Only PDF files are allowed for English document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                }
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional validation

                // Convert DateTime to UTC for PostgreSQL
                var publishDateUtc = viewModel.PublishDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishDate, DateTimeKind.Utc)
                    : viewModel.PublishDate.ToUniversalTime();

                var currentUtc = DateTime.UtcNow;

                _logger.LogInformation("DateTime conversion - Original: {Original}, UTC: {Utc}",
                    viewModel.PublishDate, publishDateUtc);

                var document = new Document
                {
                    TitleUz = viewModel.TitleUz?.Trim() ?? "",
                    TitleRu = viewModel.TitleRu?.Trim() ?? "",
                    TitleEn = viewModel.TitleEn?.Trim() ?? "",
                    DescriptionUz = viewModel.DescriptionUz?.Trim(),
                    DescriptionRu = viewModel.DescriptionRu?.Trim(),
                    DescriptionEn = viewModel.DescriptionEn?.Trim(),
                    Category = viewModel.Category,
                    Status = viewModel.Status,
                    PublishDate = publishDateUtc,
                    // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Fixed with explicit casting and default values
                    FileName = viewModel.Document?.FileName ?? "",
                    FileSizeBytes = viewModel.Document?.Length ?? 0, // Provide default value 0 if null
                    FileNameUz = viewModel.DocumentUz?.FileName ?? "",
                    FileSizeBytesUz = viewModel.DocumentUz?.Length ?? 0, // Provide default value 0 if null
                    FileNameEn = viewModel.DocumentEn?.FileName ?? "",
                    FileSizeBytesEn = viewModel.DocumentEn?.Length ?? 0, // Provide default value 0 if null
                    // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of fixed casting
                    CreatedAt = currentUtc,
                    UpdatedAt = currentUtc
                };

                // Set category translations
                var categoryTranslations = viewModel.Category.GetTranslations();
                document.CategoryUz = categoryTranslations.Uz;
                document.CategoryRu = categoryTranslations.Ru;
                document.CategoryEn = categoryTranslations.En;

                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Upload files only if provided (optional uploads)
                // Upload Uzbek file if provided
                if (viewModel.DocumentUz != null && viewModel.DocumentUz.Length > 0)
                {
                    _logger.LogInformation("AAA - Uploading Uzbek PDF document via API...");
                    document.FileUrlUz = await _apiFileService.SaveFileAsync(viewModel.DocumentUz, "documents/uz");
                    if (string.IsNullOrEmpty(document.FileUrlUz))
                    {
                        _logger.LogError("AAA - Failed to upload Uzbek PDF document via API");
                        ModelState.AddModelError("DocumentUz", "Failed to upload Uzbek PDF document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                    _logger.LogInformation("AAA - Uzbek PDF document uploaded via API: {Url}", document.FileUrlUz);
                }

                // Upload Russian file if provided
                if (viewModel.Document != null && viewModel.Document.Length > 0)
                {
                    _logger.LogInformation("AAA - Uploading Russian PDF document via API...");
                    document.FileUrl = await _apiFileService.SaveFileAsync(viewModel.Document, "documents/ru");
                    if (string.IsNullOrEmpty(document.FileUrl))
                    {
                        _logger.LogError("AAA - Failed to upload Russian PDF document via API");
                        ModelState.AddModelError("Document", "Failed to upload Russian PDF document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                    _logger.LogInformation("AAA - Russian PDF document uploaded via API: {Url}", document.FileUrl);
                }

                // Upload English file if provided
                if (viewModel.DocumentEn != null && viewModel.DocumentEn.Length > 0)
                {
                    _logger.LogInformation("AAA - Uploading English PDF document via API...");
                    document.FileUrlEn = await _apiFileService.SaveFileAsync(viewModel.DocumentEn, "documents/en");
                    if (string.IsNullOrEmpty(document.FileUrlEn))
                    {
                        _logger.LogError("AAA - Failed to upload English PDF document via API");
                        ModelState.AddModelError("DocumentEn", "Failed to upload English PDF document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        return View(viewModel);
                    }
                    _logger.LogInformation("AAA - English PDF document uploaded via API: {Url}", document.FileUrlEn);
                }

                _logger.LogInformation("AAA - Document created with optional files: UZ={UzUrl}, RU={RuUrl}, EN={EnUrl}",
                    document.FileUrlUz ?? "None", document.FileUrl ?? "None", document.FileUrlEn ?? "None");
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional file upload

                _logger.LogInformation("Adding document to context...");
                _context.Add(document);

                _logger.LogInformation("Saving changes to database...");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document saved with ID: {Id}", document.Id);
                _logger.LogInformation("=== CREATE DOCUMENT COMPLETED SUCCESSFULLY ===");

                TempData["SuccessMessage"] = "Document created successfully! Files uploaded for available languages.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document");
                ModelState.AddModelError("", $"An error occurred while creating the document: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
            return View(viewModel);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(document);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(document);
            ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // POST: Documents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentViewModel viewModel)
        {
            _logger.LogInformation("=== EDIT DOCUMENT REQUEST STARTED ===");
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

                ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
                return View(viewModel);
            }

            try
            {
                // Fetch the entity and ensure it's tracked
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    _logger.LogWarning("Document with ID {Id} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Found existing document: {Title}", document.TitleEn);

                // Convert DateTime to UTC for PostgreSQL
                var publishDateUtc = viewModel.PublishDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(viewModel.PublishDate, DateTimeKind.Utc)
                    : viewModel.PublishDate.ToUniversalTime();

                // Update properties
                document.TitleUz = viewModel.TitleUz?.Trim() ?? "";
                document.TitleRu = viewModel.TitleRu?.Trim() ?? "";
                document.TitleEn = viewModel.TitleEn?.Trim() ?? "";
                document.DescriptionUz = viewModel.DescriptionUz?.Trim();
                document.DescriptionRu = viewModel.DescriptionRu?.Trim();
                document.DescriptionEn = viewModel.DescriptionEn?.Trim();
                document.Category = viewModel.Category;
                document.Status = viewModel.Status;
                document.PublishDate = publishDateUtc;
                document.UpdatedAt = DateTime.UtcNow;

                // Update category translations
                var categoryTranslations = viewModel.Category.GetTranslations();
                document.CategoryUz = categoryTranslations.Uz;
                document.CategoryRu = categoryTranslations.Ru;
                document.CategoryEn = categoryTranslations.En;

                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Handle optional multilingual PDF file uploads via API
                // Handle Uzbek document upload (optional)
                if (viewModel.DocumentUz != null && viewModel.DocumentUz.Length > 0)
                {
                    var fileExtensionUz = Path.GetExtension(viewModel.DocumentUz.FileName).ToLowerInvariant();
                    if (fileExtensionUz != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for Uzbek document. Received: {Extension}", fileExtensionUz);
                        ModelState.AddModelError("DocumentUz", "Only PDF files are allowed for Uzbek document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        ViewBag.ApiFileService = _apiFileService;
                        return View(viewModel);
                    }

                    _logger.LogInformation("AAA - Updating Uzbek PDF document via API...");

                    // Delete old Uzbek file from API if exists
                    if (!string.IsNullOrEmpty(document.FileUrlUz))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrlUz);
                        _logger.LogInformation("AAA - Deleted old Uzbek document from API: {Url}", document.FileUrlUz);
                    }

                    // Save new Uzbek file via API
                    document.FileUrlUz = await _apiFileService.SaveFileAsync(viewModel.DocumentUz, "documents/uz");
                    document.FileNameUz = viewModel.DocumentUz.FileName;
                    document.FileSizeBytesUz = viewModel.DocumentUz.Length;

                    _logger.LogInformation("AAA - Uzbek PDF document updated via API: {Url}", document.FileUrlUz);
                }

                // Handle Russian document upload (optional)
                if (viewModel.Document != null && viewModel.Document.Length > 0)
                {
                    var fileExtensionRu = Path.GetExtension(viewModel.Document.FileName).ToLowerInvariant();
                    if (fileExtensionRu != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for Russian document. Received: {Extension}", fileExtensionRu);
                        ModelState.AddModelError("Document", "Only PDF files are allowed for Russian document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        ViewBag.ApiFileService = _apiFileService;
                        return View(viewModel);
                    }

                    _logger.LogInformation("AAA - Updating Russian PDF document via API...");

                    // Delete old Russian file from API if exists
                    if (!string.IsNullOrEmpty(document.FileUrl))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrl);
                        _logger.LogInformation("AAA - Deleted old Russian document from API: {Url}", document.FileUrl);
                    }

                    // Save new Russian file via API
                    document.FileUrl = await _apiFileService.SaveFileAsync(viewModel.Document, "documents/ru");
                    document.FileName = viewModel.Document.FileName;
                    document.FileSizeBytes = viewModel.Document.Length;

                    _logger.LogInformation("AAA - Russian PDF document updated via API: {Url}", document.FileUrl);
                }

                // Handle English document upload (optional)
                if (viewModel.DocumentEn != null && viewModel.DocumentEn.Length > 0)
                {
                    var fileExtensionEn = Path.GetExtension(viewModel.DocumentEn.FileName).ToLowerInvariant();
                    if (fileExtensionEn != ".pdf")
                    {
                        _logger.LogError("Only PDF files are allowed for English document. Received: {Extension}", fileExtensionEn);
                        ModelState.AddModelError("DocumentEn", "Only PDF files are allowed for English document.");
                        ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
                        ViewBag.ApiFileService = _apiFileService;
                        return View(viewModel);
                    }

                    _logger.LogInformation("AAA - Updating English PDF document via API...");

                    // Delete old English file from API if exists
                    if (!string.IsNullOrEmpty(document.FileUrlEn))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrlEn);
                        _logger.LogInformation("AAA - Deleted old English document from API: {Url}", document.FileUrlEn);
                    }

                    // Save new English file via API
                    document.FileUrlEn = await _apiFileService.SaveFileAsync(viewModel.DocumentEn, "documents/en");
                    document.FileNameEn = viewModel.DocumentEn.FileName;
                    document.FileSizeBytesEn = viewModel.DocumentEn.Length;

                    _logger.LogInformation("AAA - English PDF document updated via API: {Url}", document.FileUrlEn);
                }
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional multilingual file upload handling

                _logger.LogInformation("Calling SaveChangesAsync...");
                var changeCount = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed. Changes saved: {ChangeCount}", changeCount);

                _logger.LogInformation("=== EDIT DOCUMENT COMPLETED SUCCESSFULLY ===");
                TempData["SuccessMessage"] = $"Document updated successfully! Changes saved: {changeCount}";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating document {Id}", id);
                if (!DocumentExists(viewModel.Id))
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
                _logger.LogError(ex, "Error updating document {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the document: " + ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
            }

            ViewBag.Categories = DocumentCategoryExtensions.GetAllCategories();
            ViewBag.ApiFileService = _apiFileService; // AAA - Pass API file service
            return View(viewModel);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(document);
            // AAA - Pass API file service to view for URL conversion
            ViewBag.ApiFileService = _apiFileService;
            return View(viewModel);
        }

        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - KEEP THIS ONE: Updated Delete method for optional files
        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document != null)
                {
                    // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Delete all multilingual files from API (only if they exist)
                    _logger.LogInformation("AAA - Deleting associated files from API...");

                    // Delete Uzbek file if exists
                    if (!string.IsNullOrEmpty(document.FileUrlUz))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrlUz);
                        _logger.LogInformation("AAA - Deleted Uzbek document file from API: {Url}", document.FileUrlUz);
                    }
                    else
                    {
                        _logger.LogInformation("AAA - No Uzbek document file to delete");
                    }

                    // Delete Russian file if exists
                    if (!string.IsNullOrEmpty(document.FileUrl))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrl);
                        _logger.LogInformation("AAA - Deleted Russian document file from API: {Url}", document.FileUrl);
                    }
                    else
                    {
                        _logger.LogInformation("AAA - No Russian document file to delete");
                    }

                    // Delete English file if exists
                    if (!string.IsNullOrEmpty(document.FileUrlEn))
                    {
                        await _apiFileService.DeleteFileAsync(document.FileUrlEn);
                        _logger.LogInformation("AAA - Deleted English document file from API: {Url}", document.FileUrlEn);
                    }
                    else
                    {
                        _logger.LogInformation("AAA - No English document file to delete");
                    }
                    // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional multilingual file deletion

                    _context.Documents.Remove(document);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Document and all available language versions deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAA - Error deleting document");
                TempData["ErrorMessage"] = "An error occurred while deleting the document: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }

        // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - KEEP THIS ONE: Updated MapToViewModel method for optional files
        private DocumentViewModel MapToViewModel(Document document)
        {
            return new DocumentViewModel
            {
                Id = document.Id,
                TitleUz = document.TitleUz,
                TitleRu = document.TitleRu,
                TitleEn = document.TitleEn,
                DescriptionUz = document.DescriptionUz,
                DescriptionRu = document.DescriptionRu,
                DescriptionEn = document.DescriptionEn,
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Updated file properties mapping for optional files
                FileUrl = document.FileUrl, // Russian (keeping original)
                FileName = document.FileName,
                FileSizeBytes = document.FileSizeBytes,
                FileUrlUz = document.FileUrlUz, // Uzbek
                FileNameUz = document.FileNameUz,
                FileSizeBytesUz = document.FileSizeBytesUz,
                FileUrlEn = document.FileUrlEn, // English
                FileNameEn = document.FileNameEn,
                FileSizeBytesEn = document.FileSizeBytesEn,
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional file properties
                Category = document.Category,
                Status = document.Status,
                PublishDate = document.PublishDate,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt
            };
        }
    }
}