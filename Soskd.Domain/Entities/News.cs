// Soskd.Domain/Entities/News.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;
namespace Soskd.Domain.Entities
{
    public class News
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;
        [Required]
        public string DescriptionUz { get; set; } = string.Empty;
        [Required]
        public string DescriptionRu { get; set; } = string.Empty;
        [Required]
        public string DescriptionEn { get; set; } = string.Empty;
        // Slugs (Optional)
        [MaxLength(500)]
        public string? SlugUz { get; set; }
        [MaxLength(500)]
        public string? SlugRu { get; set; }
        [MaxLength(500)]
        public string? SlugEn { get; set; }
        // Meta Titles (Optional)
        [MaxLength(500)]
        public string? MetaTitleUz { get; set; }
        [MaxLength(500)]
        public string? MetaTitleRu { get; set; }
        [MaxLength(500)]
        public string? MetaTitleEn { get; set; }
        // Meta Descriptions (Optional)
        [MaxLength(1000)]
        public string? MetaDescriptionUz { get; set; }
        [MaxLength(1000)]
        public string? MetaDescriptionRu { get; set; }
        [MaxLength(1000)]
        public string? MetaDescriptionEn { get; set; }
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        [Required]
        public NewsStatus Status { get; set; } = NewsStatus.Published;
        [Required]
        [MaxLength(500)]
        public string SmallPhotoUrl { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? LargePhotoUrl { get; set; }
        [Required]
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}