// Soskd.Domain/Entities/Document.cs
using System.ComponentModel.DataAnnotations;
using Soskd.Domain.Enums;
namespace Soskd.Domain.Entities
{
    public class Document
    {
        public int Id { get; set; }
        // Titles
        [Required]
        [MaxLength(500)]
        public string TitleUz { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleRu { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions (Optional)
        public string? DescriptionUz { get; set; }
        public string? DescriptionRu { get; set; }
        public string? DescriptionEn { get; set; }

        // File information
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;
        public long? FileSizeBytes { get; set; }
        public string FileSizeFormatted => FormatFileSize(FileSizeBytes ?? 0);

        // File information - Uzbek
        [MaxLength(500)]
        public string FileUrlUz { get; set; } = string.Empty;
        [MaxLength(100)]
        public string FileNameUz { get; set; } = string.Empty;
        public long? FileSizeBytesUz { get; set; }
        public string FileSizeFormattedUz => FormatFileSize(FileSizeBytesUz ?? 0);

        // File information - English
        [MaxLength(500)]
        public string FileUrlEn { get; set; } = string.Empty;
        [MaxLength(100)]
        public string FileNameEn { get; set; } = string.Empty;
        public long? FileSizeBytesEn { get; set; }
        public string FileSizeFormattedEn => FormatFileSize(FileSizeBytesEn ?? 0);

        // Category
        [Required]
        public DocumentCategory Category { get; set; } = DocumentCategory.Documents;
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        // Status and dates
        [Required]
        public DocumentStatus Status { get; set; } = DocumentStatus.Published;
        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }
    }
}