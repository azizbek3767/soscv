// Soskd.Admin/Controllers/HomeController.cs - Updated with VacancyApplication statistics
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soskd.Admin.Models;
using Soskd.Domain.Enums;
using Soskd.Infrastructure.Data;
using System.Diagnostics;

namespace Soskd.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUtc = DateTime.UtcNow;
                var thisMonthStart = new DateTime(currentUtc.Year, currentUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var thisWeekStart = currentUtc.AddDays(-(int)currentUtc.DayOfWeek);

                // News Statistics
                ViewBag.TotalNews = await _context.News.CountAsync();
                ViewBag.PublishedNews = await _context.News.CountAsync(n => n.Status == NewsStatus.Published);
                ViewBag.UnpublishedNews = await _context.News.CountAsync(n => n.Status == NewsStatus.Unpublished);
                ViewBag.ThisMonthNews = await _context.News.CountAsync(n => n.CreatedAt >= thisMonthStart);

                // Vacancy Statistics
                ViewBag.TotalVacancies = await _context.Vacancies.CountAsync();
                ViewBag.ActiveVacancies = await _context.Vacancies.CountAsync(v =>
                    v.Status == VacancyStatus.Published &&
                    (v.Deadline == null || v.Deadline > currentUtc));
                ViewBag.DraftVacancies = await _context.Vacancies.CountAsync(v => v.Status == VacancyStatus.Draft);
                ViewBag.ExpiredVacancies = await _context.Vacancies.CountAsync(v =>
                    v.Deadline.HasValue && v.Deadline < currentUtc);
                ViewBag.ExpiringThisWeek = await _context.Vacancies.CountAsync(v =>
                    v.Status == VacancyStatus.Published &&
                    v.Deadline.HasValue &&
                    v.Deadline >= currentUtc &&
                    v.Deadline <= currentUtc.AddDays(7));
                ViewBag.ThisMonthVacancies = await _context.Vacancies.CountAsync(v => v.CreatedAt >= thisMonthStart);

                // NEW: VacancyApplication Statistics
                ViewBag.TotalApplications = await _context.VacancyApplications.CountAsync();
                ViewBag.ThisMonthApplications = await _context.VacancyApplications.CountAsync(a => a.CreatedAt >= thisMonthStart);
                ViewBag.ThisWeekApplications = await _context.VacancyApplications.CountAsync(a => a.CreatedAt >= thisWeekStart);
                ViewBag.TodayApplications = await _context.VacancyApplications.CountAsync(a => a.CreatedAt.Date == currentUtc.Date);

                // Document Statistics
                ViewBag.TotalDocuments = await _context.Documents.CountAsync();
                ViewBag.PublishedDocuments = await _context.Documents.CountAsync(d => d.Status == DocumentStatus.Published);
                ViewBag.UnpublishedDocuments = await _context.Documents.CountAsync(d => d.Status == DocumentStatus.Unpublished);
                ViewBag.DocumentsCategory = await _context.Documents.CountAsync(d => d.Category == DocumentCategory.Documents);
                ViewBag.SecurityAndValuesCategory = await _context.Documents.CountAsync(d => d.Category == DocumentCategory.SecurityAndValues);
                ViewBag.ThisMonthDocuments = await _context.Documents.CountAsync(d => d.CreatedAt >= thisMonthStart);
                ViewBag.RecentDocuments = await _context.Documents.CountAsync(d => d.CreatedAt >= thisWeekStart);

                // Media About Us Statistics
                ViewBag.TotalMediaAboutUs = await _context.MediaAboutUs.CountAsync();
                ViewBag.PublishedMediaAboutUs = await _context.MediaAboutUs.CountAsync(m => m.Status == MediaStatus.Published);
                ViewBag.UnpublishedMediaAboutUs = await _context.MediaAboutUs.CountAsync(m => m.Status == MediaStatus.Unpublished);
                ViewBag.ThisMonthMediaAboutUs = await _context.MediaAboutUs.CountAsync(m => m.CreatedAt >= thisMonthStart);
                ViewBag.RecentMediaAboutUs = await _context.MediaAboutUs.CountAsync(m => m.CreatedAt >= thisWeekStart);

                // Sponsors Statistics
                ViewBag.TotalSponsors = await _context.Sponsors.CountAsync();
                ViewBag.PublishedSponsors = await _context.Sponsors.CountAsync(s => s.Status == SponsorStatus.Published);
                ViewBag.UnpublishedSponsors = await _context.Sponsors.CountAsync(s => s.Status == SponsorStatus.Unpublished);
                ViewBag.ThisMonthSponsors = await _context.Sponsors.CountAsync(s => s.CreatedAt >= thisMonthStart);
                ViewBag.RecentSponsors = await _context.Sponsors.CountAsync(s => s.CreatedAt >= thisWeekStart);


                ViewBag.TotalContactApplications = await _context.ContactApplications.CountAsync();
                ViewBag.ThisMonthContactApplications = await _context.ContactApplications.CountAsync(c => c.CreatedAt >= thisMonthStart);
                ViewBag.ThisWeekContactApplications = await _context.ContactApplications.CountAsync(c => c.CreatedAt >= thisWeekStart);
                ViewBag.TodayContactApplications = await _context.ContactApplications.CountAsync(c => c.CreatedAt.Date == currentUtc.Date);


                ViewBag.TotalDonations = await _context.Donations.CountAsync();
                ViewBag.CompletedDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Completed);
                ViewBag.PendingDonations = await _context.Donations.CountAsync(d => d.Status == DonationStatus.Pending);
                ViewBag.ThisMonthDonations = await _context.Donations.CountAsync(d => d.CreatedAt >= thisMonthStart);
                ViewBag.TotalDonationAmount = await _context.Donations
                    .Where(d => d.Status == DonationStatus.Completed)
                    .SumAsync(d => d.ProcessedAmount ?? d.Amount);
                ViewBag.ThisMonthDonationAmount = await _context.Donations
                    .Where(d => d.Status == DonationStatus.Completed && d.PaymentCompletedAt >= thisMonthStart)
                    .SumAsync(d => d.ProcessedAmount ?? d.Amount);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics");
                ViewBag.ErrorMessage = "Unable to load dashboard statistics. Please try again.";

                // Set default values to prevent view errors
                ViewBag.TotalNews = 0;
                ViewBag.PublishedNews = 0;
                ViewBag.UnpublishedNews = 0;
                ViewBag.ThisMonthNews = 0;
                ViewBag.TotalVacancies = 0;
                ViewBag.ActiveVacancies = 0;
                ViewBag.DraftVacancies = 0;
                ViewBag.ExpiredVacancies = 0;
                ViewBag.ExpiringThisWeek = 0;
                ViewBag.ThisMonthVacancies = 0;
                ViewBag.TotalApplications = 0;
                ViewBag.ThisMonthApplications = 0;
                ViewBag.ThisWeekApplications = 0;
                ViewBag.TodayApplications = 0;
                ViewBag.TotalDocuments = 0;
                ViewBag.PublishedDocuments = 0;
                ViewBag.UnpublishedDocuments = 0;
                ViewBag.DocumentsCategory = 0;
                ViewBag.SecurityAndValuesCategory = 0;
                ViewBag.ThisMonthDocuments = 0;
                ViewBag.RecentDocuments = 0;
                ViewBag.TotalMediaAboutUs = 0;
                ViewBag.PublishedMediaAboutUs = 0;
                ViewBag.UnpublishedMediaAboutUs = 0;
                ViewBag.ThisMonthMediaAboutUs = 0;
                ViewBag.RecentMediaAboutUs = 0;
                ViewBag.TotalSponsors = 0;
                ViewBag.PublishedSponsors = 0;
                ViewBag.UnpublishedSponsors = 0;
                ViewBag.ThisMonthSponsors = 0;
                ViewBag.RecentSponsors = 0;
                ViewBag.TotalContactApplications = 0;
                ViewBag.ThisMonthContactApplications = 0;
                ViewBag.ThisWeekContactApplications = 0;
                ViewBag.TodayContactApplications = 0;

                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

























//using System.Diagnostics;
//using Microsoft.AspNetCore.Mvc;
//using Soskd.Admin.Models;

//namespace Soskd.Admin.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        public HomeController(ILogger<HomeController> logger)
//        {
//            _logger = logger;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
