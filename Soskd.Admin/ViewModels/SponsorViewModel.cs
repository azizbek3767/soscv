// Soskd.Admin/ViewModels/SponsorViewModel.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Admin.ViewModels
{
    public class SponsorViewModel
    {
        public int Id { get; set; }

        // Titles in three languages
        [Required(ErrorMessage = "Title in Uzbek is required")]
        [Display(Name = "Title (Uzbek)")]
        [MaxLength(200)]
        public string TitleUz { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in Russian is required")]
        [Display(Name = "Title (Russian)")]
        [MaxLength(200)]
        public string TitleRu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title in English is required")]
        [Display(Name = "Title (English)")]
        [MaxLength(200)]
        public string TitleEn { get; set; } = string.Empty;

        // Photo upload
        [Display(Name = "Logo/Photo")]
        public IFormFile? Photo { get; set; }

        public string? PhotoUrl { get; set; }

        // Website link
        [Required(ErrorMessage = "Website link is required")]
        [Display(Name = "Website Link")]
        [MaxLength(500)]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Link { get; set; } = string.Empty;

        // Display order
        [Display(Name = "Display Order")]
        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DisplayOrder { get; set; } = 0;

        // Status
        [Required]
        [Display(Name = "Status")]
        public SponsorStatus Status { get; set; } = SponsorStatus.Published;

        // Audit fields (read-only)
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SponsorListViewModel
    {
        public List<SponsorItemViewModel> Sponsors { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class SponsorItemViewModel
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public SponsorStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}