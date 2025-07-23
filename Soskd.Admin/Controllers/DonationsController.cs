// Soskd.Admin DonationsController
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.ViewModels;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DonationsController> _logger;

        public DonationsController(ApplicationDbContext context, ILogger<DonationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 20,
            string? status = null,
            string? type = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? searchTerm = null)
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Min(100, Math.Max(10, pageSize));

                var query = _context.Donations.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<DonationStatus>(status, out var statusEnum))
                {
                    query = query.Where(d => d.Status == statusEnum);
                }

                if (!string.IsNullOrEmpty(type) && Enum.TryParse<DonationType>(type, out var typeEnum))
                {
                    query = query.Where(d => d.Type == typeEnum);
                }

                if (dateFrom.HasValue)
                {
                    query = query.Where(d => d.CreatedAt >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    query = query.Where(d => d.CreatedAt <= dateTo.Value.AddDays(1));
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    query = query.Where(d =>
                        d.DonorName.ToLower().Contains(searchLower) ||
                        d.DonorEmail.ToLower().Contains(searchLower) ||
                        d.OrderId.ToLower().Contains(searchLower) ||
                        (d.UpayTransactionId != null && d.UpayTransactionId.ToLower().Contains(searchLower)));
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var donations = await query
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new DonationViewModel
                    {
                        Id = d.Id,
                        DonorName = d.DonorName,
                        DonorEmail = d.DonorEmail,
                        DonorPhone = d.DonorPhone,
                        Amount = d.Amount,
                        Type = d.Type,
                        Status = d.Status,
                        OrderId = d.OrderId,
                        UpayTransactionId = d.UpayTransactionId,
                        PaymentMethod = d.PaymentMethod,
                        FailureReason = d.FailureReason,
                        PaymentCompletedAt = d.PaymentCompletedAt,
                        NextPaymentDate = d.NextPaymentDate,
                        IsRecurring = d.IsRecurring,
                        ParentDonationId = d.ParentDonationId,
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    })
                    .ToListAsync();

                var viewModel = new DonationListViewModel
                {
                    Donations = donations,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    StatusFilter = status,
                    TypeFilter = type,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    SearchTerm = searchTerm
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donations index");
                TempData["ErrorMessage"] = "Error loading donations: " + ex.Message;
                return View(new DonationListViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var donation = await _context.Donations
                    .Include(d => d.ParentDonation)
                    .Include(d => d.RecurringPayments)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (donation == null)
                {
                    return NotFound();
                }

                var viewModel = new DonationViewModel
                {
                    Id = donation.Id,
                    DonorName = donation.DonorName,
                    DonorEmail = donation.DonorEmail,
                    DonorPhone = donation.DonorPhone,
                    Amount = donation.Amount,
                    Type = donation.Type,
                    Status = donation.Status,
                    OrderId = donation.OrderId,
                    UpayTransactionId = donation.UpayTransactionId,
                    PaymentMethod = donation.PaymentMethod,
                    FailureReason = donation.FailureReason,
                    PaymentCompletedAt = donation.PaymentCompletedAt,
                    NextPaymentDate = donation.NextPaymentDate,
                    IsRecurring = donation.IsRecurring,
                    ParentDonationId = donation.ParentDonationId,
                    CreatedAt = donation.CreatedAt,
                    UpdatedAt = donation.UpdatedAt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donation details for id {Id}", id);
                TempData["ErrorMessage"] = "Error loading donation details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Statistics()
        {
            try
            {
                var currentUtc = DateTime.UtcNow;
                var thisMonthStart = new DateTime(currentUtc.Year, currentUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);

                var stats = new DonationStatsViewModel
                {
                    TotalDonations = await _context.Donations.CountAsync(),
                    CompletedDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Completed),
                    PendingDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Pending),
                    FailedDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Failed),
                    TotalAmount = await _context.Donations
                        .Where(d => d.Status == DonationStatus.Completed)
                        .SumAsync(d => d.ProcessedAmount ?? d.Amount),
                    ThisMonthAmount = await _context.Donations
                        .Where(d => d.Status == DonationStatus.Completed && d.PaymentCompletedAt >= thisMonthStart)
                        .SumAsync(d => d.ProcessedAmount ?? d.Amount),
                    MonthlyDonations = await _context.Donations.CountAsync(d => d.Type == DonationType.Monthly),
                    OneTimeDonations = await _context.Donations.CountAsync(d => d.Type == DonationType.OneTime),
                    AverageAmount = await _context.Donations
                        .Where(d => d.Status == DonationStatus.Completed)
                        .AverageAsync(d => d.ProcessedAmount ?? d.Amount)
                };

                // Get monthly statistics for the last 12 months
                var monthlyStats = await _context.Donations
                    .Where(d => d.Status == DonationStatus.Completed &&
                               d.PaymentCompletedAt >= currentUtc.AddMonths(-11))
                    .GroupBy(d => new {
                        Year = d.PaymentCompletedAt!.Value.Year,
                        Month = d.PaymentCompletedAt!.Value.Month
                    })
                    .Select(g => new MonthlyStatsItem
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Amount = g.Sum(d => d.ProcessedAmount ?? d.Amount),
                        Count = g.Count()
                    })
                    .OrderBy(s => s.Month)
                    .ToListAsync();

                stats.MonthlyStats = monthlyStats;

                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donation statistics");
                TempData["ErrorMessage"] = "Error loading statistics: " + ex.Message;
                return View(new DonationStatsViewModel());
            }
        }
    }
}