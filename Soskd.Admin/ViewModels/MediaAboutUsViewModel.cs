// Soskd.Admin/ViewModels/MediaAboutUsViewModel.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Admin.ViewModels
{
    public class MediaAboutUsViewModel
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

        // Photo
        [Display(Name = "Photo")]
        public IFormFile? Photo { get; set; }

        public string? PhotoUrl { get; set; }

        // Source Link
        [Required(ErrorMessage = "Source link is required")]
        [Display(Name = "Source Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [MaxLength(1000)]
        public string SourceLink { get; set; } = string.Empty;

        // Status
        [Required]
        [Display(Name = "Status")]
        public MediaStatus Status { get; set; } = MediaStatus.Published;

        // Publish Date
        [Required]
        [Display(Name = "Publish Date")]
        [DataType(DataType.DateTime)]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        // Audit fields
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Helper properties
        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    MediaStatus.Published => "Published",
                    MediaStatus.Unpublished => "Unpublished",
                    _ => "Unknown"
                };
            }
        }
    }

    public class MediaAboutUsListViewModel
    {
        public List<MediaAboutUsItemViewModel> MediaItems { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class MediaAboutUsItemViewModel
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        public MediaStatus Status { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Helper properties
        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    MediaStatus.Published => "Published",
                    MediaStatus.Unpublished => "Unpublished",
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
                    MediaStatus.Published => "status-published",
                    MediaStatus.Unpublished => "status-unpublished",
                    _ => "status-unknown"
                };
            }
        }
    }
}