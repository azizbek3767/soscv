// Soskd.Admin/Controllers/ContactApplicationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Infrastructure.Data;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ContactApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactApplicationsController> _logger;

        public ContactApplicationsController(ApplicationDbContext context, ILogger<ContactApplicationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ContactApplications
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? search = null)
        {
            try
            {
                _logger.LogInformation("Loading contact applications index page {Page}", page);

                var query = _context.ContactApplications.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.FullName.Contains(search) ||
                                           c.Email.Contains(search) ||
                                           c.City.Contains(search) ||
                                           c.Message.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var applications = await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new ContactApplicationItemViewModel
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        City = c.City,
                        MessagePreview = c.Message.Length > 100 ? c.Message.Substring(0, 100) + "..." : c.Message,
                        CreatedAt = c.CreatedAt,
                        IpAddress = c.IpAddress
                    })
                    .ToListAsync();

                var viewModel = new ContactApplicationListViewModel
                {
                    Applications = applications,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    SearchQuery = search
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading contact applications index");
                TempData["ErrorMessage"] = $"Error loading contact applications: {ex.Message}";
                return View(new ContactApplicationListViewModel { Applications = new List<ContactApplicationItemViewModel>() });
            }
        }

        // GET: ContactApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var application = await _context.ContactApplications
                    .Where(c => c.Id == id)
                    .Select(c => new ContactApplicationViewModel
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        City = c.City,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt,
                        IpAddress = c.IpAddress,
                        UserAgent = c.UserAgent
                    })
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    return NotFound();
                }

                return View(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading contact application details for ID {Id}", id);
                TempData["ErrorMessage"] = $"Error loading application details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ContactApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var application = await _context.ContactApplications
                    .Where(c => c.Id == id)
                    .Select(c => new ContactApplicationViewModel
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        City = c.City,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt,
                        IpAddress = c.IpAddress
                    })
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    return NotFound();
                }

                return View(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading contact application for deletion, ID {Id}", id);
                TempData["ErrorMessage"] = $"Error loading application: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ContactApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var application = await _context.ContactApplications.FindAsync(id);
                if (application != null)
                {
                    _logger.LogInformation("Deleting contact application {Id} from {Email}", id, application.Email);

                    _context.ContactApplications.Remove(application);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Contact application deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Contact application not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact application {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the application: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ContactApplications/BulkDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromForm] int[] selectedIds)
        {
            try
            {
                if (selectedIds == null || selectedIds.Length == 0)
                {
                    TempData["ErrorMessage"] = "No applications selected for deletion.";
                    return RedirectToAction(nameof(Index));
                }

                var applications = await _context.ContactApplications
                    .Where(c => selectedIds.Contains(c.Id))
                    .ToListAsync();

                if (applications.Any())
                {
                    _logger.LogInformation("Bulk deleting {Count} contact applications", applications.Count);

                    _context.ContactApplications.RemoveRange(applications);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Successfully deleted {applications.Count} contact applications.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No applications found for deletion.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk delete of contact applications");
                TempData["ErrorMessage"] = "An error occurred during bulk deletion: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ContactApplications/Export
        public async Task<IActionResult> Export()
        {
            try
            {
                var applications = await _context.ContactApplications
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var csv = "ID,Full Name,Email,Phone,City,Message,Created At,IP Address\n";
                foreach (var app in applications)
                {
                    csv += $"{app.Id}," +
                           $"\"{app.FullName}\"," +
                           $"\"{app.Email}\"," +
                           $"\"{app.Phone ?? ""}\"," +
                           $"\"{app.City}\"," +
                           $"\"{app.Message.Replace("\"", "\"\"")}\"," +
                           $"\"{app.CreatedAt:yyyy-MM-dd HH:mm:ss}\"," +
                           $"\"{app.IpAddress ?? ""}\"\n";
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                var fileName = $"contact_applications_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting contact applications");
                TempData["ErrorMessage"] = "Error exporting applications: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}