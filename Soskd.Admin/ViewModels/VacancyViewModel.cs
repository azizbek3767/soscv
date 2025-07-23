// Soskd.Admin/ViewModels/VacancyViewModel.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;
using Soskd.Domain.Validation;

namespace Soskd.Admin.ViewModels
{
    public class VacancyViewModel
    {
        public int Id { get; set; }

        // Uzbek Content
        [Required(ErrorMessage = "Title in Uzbek is required")]
        [Display(Name = "Title (Uzbek)")]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in Uzbek is required")]
        [Display(Name = "Description (Uzbek)")]
        public string DescriptionUz { get; set; } = string.Empty;

        // Russian Content
        [Required(ErrorMessage = "Title in Russian is required")]
        [Display(Name = "Title (Russian)")]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in Russian is required")]
        [Display(Name = "Description (Russian)")]
        public string DescriptionRu { get; set; } = string.Empty;

        // English Content
        [Required(ErrorMessage = "Title in English is required")]
        [Display(Name = "Title (English)")]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in English is required")]
        [Display(Name = "Description (English)")]
        public string DescriptionEn { get; set; } = string.Empty;

        // Status (manageable from admin)
        [Required]
        [Display(Name = "Status")]
        public VacancyStatus Status { get; set; } = VacancyStatus.Published;

        // Optional Dates
        [Display(Name = "Published Date")]
        [DataType(DataType.DateTime)]
        public DateTime? PublishedDate { get; set; }

        [Display(Name = "Application Deadline")]
        [DataType(DataType.DateTime)]
        public DateTime? Deadline { get; set; }

        // Audit fields
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Helper properties
        public bool IsPublished => Status == VacancyStatus.Published && (!PublishedDate.HasValue || PublishedDate.Value <= DateTime.UtcNow);
        public bool IsExpired => Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        public bool IsActive => IsPublished && !IsExpired && Status != VacancyStatus.Closed;
        public bool IsClosed => Status == VacancyStatus.Closed;

        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    VacancyStatus.Draft => "Draft",
                    VacancyStatus.Published => IsExpired ? "Expired" : "Active",
                    VacancyStatus.Closed => "Closed",
                    _ => "Unknown"
                };
            }
        }
    }

    public class VacancyListViewModel
    {
        public List<VacancyItemViewModel> Vacancies { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class VacancyItemViewModel
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public VacancyStatus Status { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }

        // Helper properties
        public bool IsPublished => Status == VacancyStatus.Published && (!PublishedDate.HasValue || PublishedDate.Value <= DateTime.UtcNow);
        public bool IsExpired => Deadline.HasValue && Deadline.Value < DateTime.UtcNow;
        public bool IsActive => IsPublished && !IsExpired && Status != VacancyStatus.Closed;
        public bool IsClosed => Status == VacancyStatus.Closed;

        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    VacancyStatus.Draft => "Draft",
                    VacancyStatus.Published => IsExpired ? "Expired" : "Active",
                    VacancyStatus.Closed => "Closed",
                    _ => "Unknown"
                };
            }
        }

        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    VacancyStatus.Draft => "status-draft",
                    VacancyStatus.Published => IsExpired ? "status-expired" : "status-active",
                    VacancyStatus.Closed => "status-closed",
                    _ => "status-unknown"
                };
            }
        }
    }
}


