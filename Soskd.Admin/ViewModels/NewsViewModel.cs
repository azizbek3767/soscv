// Soskd.Admin/ViewModels/NewsViewModel.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;

namespace Soskd.Admin.ViewModels
{
    public class NewsViewModel
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

        [Display(Name = "Slug (Uzbek)")]
        [MaxLength(500)]
        public string? SlugUz { get; set; }

        [Display(Name = "Meta Title (Uzbek)")]
        [MaxLength(500)]
        public string? MetaTitleUz { get; set; }

        [Display(Name = "Meta Description (Uzbek)")]
        [MaxLength(1000)]
        public string? MetaDescriptionUz { get; set; }

        // Russian Content
        [Required(ErrorMessage = "Title in Russian is required")]
        [Display(Name = "Title (Russian)")]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in Russian is required")]
        [Display(Name = "Description (Russian)")]
        public string DescriptionRu { get; set; } = string.Empty;

        [Display(Name = "Slug (Russian)")]
        [MaxLength(500)]
        public string? SlugRu { get; set; }

        [Display(Name = "Meta Title (Russian)")]
        [MaxLength(500)]
        public string? MetaTitleRu { get; set; }

        [Display(Name = "Meta Description (Russian)")]
        [MaxLength(1000)]
        public string? MetaDescriptionRu { get; set; }

        // English Content
        [Required(ErrorMessage = "Title in English is required")]
        [Display(Name = "Title (English)")]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description in English is required")]
        [Display(Name = "Description (English)")]
        public string DescriptionEn { get; set; } = string.Empty;

        [Display(Name = "Slug (English)")]
        [MaxLength(500)]
        public string? SlugEn { get; set; }

        [Display(Name = "Meta Title (English)")]
        [MaxLength(500)]
        public string? MetaTitleEn { get; set; }

        [Display(Name = "Meta Description (English)")]
        [MaxLength(1000)]
        public string? MetaDescriptionEn { get; set; }

        // Common Properties
        [Display(Name = "Category")]
        public NewsCategory? Category { get; set; }

        [Required]
        [Display(Name = "Status")]
        public NewsStatus Status { get; set; } = NewsStatus.Published;

        [Display(Name = "Small Photo")]
        public IFormFile? SmallPhoto { get; set; }

        [Display(Name = "Large Photo")]
        public IFormFile? LargePhoto { get; set; }

        public string? SmallPhotoUrl { get; set; }
        public string? LargePhotoUrl { get; set; }

        [Required]
        [Display(Name = "Published Date")]
        [DataType(DataType.DateTime)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Helper properties for display
        public string CategoryDisplayUz => Category?.GetTranslations().Uz ?? "";
        public string CategoryDisplayRu => Category?.GetTranslations().Ru ?? "";
        public string CategoryDisplayEn => Category?.GetTranslations().En ?? "";
    }

    public class NewsListViewModel
    {
        public List<NewsItemViewModel> News { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class NewsItemViewModel
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SlugUz { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugEn { get; set; }
        public string SmallPhotoUrl { get; set; } = string.Empty;
        public NewsStatus Status { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CategoryDisplayEn { get; set; } = string.Empty;
    }
}