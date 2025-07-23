namespace Soskd.Api.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions
        public string? DescriptionUz { get; set; }
        public string? DescriptionRu { get; set; }
        public string? DescriptionEn { get; set; }
        // File information - Russian (optional)
        public string? FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public long? FileSizeBytes { get; set; }
        public string? FileSizeFormatted { get; set; } = string.Empty;

        // File information - Uzbek (optional)
        public string? FileUrlUz { get; set; } = string.Empty;
        public string? FileNameUz { get; set; } = string.Empty;
        public long? FileSizeBytesUz { get; set; }
        public string? FileSizeFormattedUz { get; set; } = string.Empty;

        // File information - English (optional)
        public string? FileUrlEn { get; set; } = string.Empty;
        public string? FileNameEn { get; set; } = string.Empty;
        public long? FileSizeBytesEn { get; set; }
        public string? FileSizeFormattedEn { get; set; } = string.Empty;
        // Category
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        // Status and dates
        public string Status { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
    }
    public class DocumentListDto
    {
        public List<DocumentItemDto> Documents { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class DocumentItemDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions (truncated for list view)
        public string? DescriptionUz { get; set; }
        public string? DescriptionRu { get; set; }
        public string? DescriptionEn { get; set; }
        // File information - Russian (optional)
        public string? FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public long? FileSizeBytes { get; set; }
        public string? FileSizeFormatted { get; set; } = string.Empty;

        // File information - Uzbek (optional)
        public string? FileUrlUz { get; set; } = string.Empty;
        public string? FileNameUz { get; set; } = string.Empty;
        public long? FileSizeBytesUz { get; set; }
        public string? FileSizeFormattedUz { get; set; } = string.Empty;

        // File information - English (optional)
        public string? FileUrlEn { get; set; } = string.Empty;
        public string? FileNameEn { get; set; } = string.Empty;
        public long? FileSizeBytesEn { get; set; }
        public string? FileSizeFormattedEn { get; set; } = string.Empty;
        // Category
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        // Status and dates
        public DateTime PublishDate { get; set; }
    }
    public class DocumentCategoryDto
    {
        public string Uz { get; set; } = string.Empty;
        public string Ru { get; set; } = string.Empty;
        public string En { get; set; } = string.Empty;
    }
    public class DocumentSummaryDto
    {
        public int Id { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
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
        public DateTime PublishDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

