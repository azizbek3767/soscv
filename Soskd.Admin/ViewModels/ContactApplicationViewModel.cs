// Soskd.Admin/ViewModels/ContactApplicationViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Soskd.Admin.ViewModels
{
    public class ContactApplicationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Submitted At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        [Display(Name = "User Agent")]
        public string? UserAgent { get; set; }

        // Helper properties
        public string MessagePreview => Message.Length > 100 ? Message.Substring(0, 100) + "..." : Message;
        public string CreatedAtFormatted => CreatedAt.ToString("MMM dd, yyyy HH:mm");
        public bool HasPhone => !string.IsNullOrEmpty(Phone);
    }

    public class ContactApplicationItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string City { get; set; } = string.Empty;
        public string MessagePreview { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? IpAddress { get; set; }

        // Helper properties
        public bool HasPhone => !string.IsNullOrEmpty(Phone);
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                if (timeSpan.TotalDays > 30)
                    return $"{(int)(timeSpan.TotalDays / 30)} month(s) ago";
                if (timeSpan.TotalDays > 1)
                    return $"{(int)timeSpan.TotalDays} day(s) ago";
                if (timeSpan.TotalHours > 1)
                    return $"{(int)timeSpan.TotalHours} hour(s) ago";
                if (timeSpan.TotalMinutes > 1)
                    return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
                return "Just now";
            }
        }
    }

    public class ContactApplicationListViewModel
    {
        public List<ContactApplicationItemViewModel> Applications { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchQuery { get; set; }

        // Helper properties
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int StartIndex => (PageNumber - 1) * PageSize + 1;
        public int EndIndex => Math.Min(PageNumber * PageSize, TotalCount);
    }
}