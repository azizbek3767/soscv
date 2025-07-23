using Soskd.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Soskd.Admin.ViewModels
{
    public class DocumentViewModel
    {
        public int Id { get; set; }

        // Uzbek Content
        [Required(ErrorMessage = "Title in Uzbek is required")]
        [Display(Name = "Title (Uzbek)")]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;

        [Display(Name = "Description (Uzbek)")]
        public string? DescriptionUz { get; set; }

        // Russian Content
        [Required(ErrorMessage = "Title in Russian is required")]
        [Display(Name = "Title (Russian)")]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;

        [Display(Name = "Description (Russian)")]
        public string? DescriptionRu { get; set; }

        // English Content
        [Required(ErrorMessage = "Title in English is required")]
        [Display(Name = "Title (English)")]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;

        [Display(Name = "Description (English)")]
        public string? DescriptionEn { get; set; }

        // File
        [Display(Name = "PDF Document (Russian)")]
        public IFormFile? Document { get; set; }

        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string FileSizeFormatted => FormatFileSize(FileSizeBytes ?? 0);
        // File - Uzbek
        [Display(Name = "PDF Document (Uzbek)")]
        public IFormFile? DocumentUz { get; set; }

        public string? FileUrlUz { get; set; }
        public string? FileNameUz { get; set; }
        public long? FileSizeBytesUz { get; set; }
        public string FileSizeFormattedUz => FormatFileSize(FileSizeBytesUz ?? 0);

        // File - English
        [Display(Name = "PDF Document (English)")]
        public IFormFile? DocumentEn { get; set; }

        public string? FileUrlEn { get; set; }
        public string? FileNameEn { get; set; }
        public long? FileSizeBytesEn { get; set; }
        public string FileSizeFormattedEn => FormatFileSize(FileSizeBytesEn ?? 0);

        // Category
        [Required]
        [Display(Name = "Category")]
        public DocumentCategory Category { get; set; } = DocumentCategory.Documents;

        // Status
        [Required]
        [Display(Name = "Status")]
        public DocumentStatus Status { get; set; } = DocumentStatus.Published;

        // Publish Date
        [Required]
        [Display(Name = "Publish Date")]
        [DataType(DataType.DateTime)]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        // Audit fields
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Helper properties
        public string CategoryDisplayUz => Category.GetTranslations().Uz;
        public string CategoryDisplayRu => Category.GetTranslations().Ru;
        public string CategoryDisplayEn => Category.GetTranslations().En;

        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    DocumentStatus.Published => "Published",
                    DocumentStatus.Unpublished => "Unpublished",
                    _ => "Unknown"
                };
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }
    }

    public class DocumentListViewModel
    {
        public List<DocumentItemViewModel> Documents { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class DocumentItemViewModel
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? DescriptionUz { get; set; }
        public string? DescriptionRu { get; set; }
        public string? DescriptionEn { get; set; }
        // File information - Russian (optional)
        public string? FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public string? FileSizeFormatted { get; set; } = string.Empty;

        // File information - Uzbek (optional)
        public string? FileUrlUz { get; set; } = string.Empty;
        public string? FileNameUz { get; set; } = string.Empty;
        public string? FileSizeFormattedUz { get; set; } = string.Empty;

        // File information - English (optional)
        public string? FileUrlEn { get; set; } = string.Empty;
        public string? FileNameEn { get; set; } = string.Empty;
        public string? FileSizeFormattedEn { get; set; } = string.Empty;
        public DocumentStatus Status { get; set; }
        public DocumentCategory Category { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CategoryDisplayEn => Category.GetTranslations().En;

        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    DocumentStatus.Published => "Published",
                    DocumentStatus.Unpublished => "Unpublished",
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
                    DocumentStatus.Published => "status-published",
                    DocumentStatus.Unpublished => "status-unpublished",
                    _ => "status-unknown"
                };
            }
        }
    }
}
