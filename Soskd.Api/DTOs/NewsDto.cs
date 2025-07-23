// Soskd.Api/DTOs/NewsDto.cs
namespace Soskd.Api.DTOs
{
    public class NewsDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions
        public string DescriptionUz { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        // Slugs
        public string? SlugUz { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugEn { get; set; }
        // Meta Titles
        public string? MetaTitleUz { get; set; }
        public string? MetaTitleRu { get; set; }
        public string? MetaTitleEn { get; set; }
        // Meta Descriptions
        public string? MetaDescriptionUz { get; set; }
        public string? MetaDescriptionRu { get; set; }
        public string? MetaDescriptionEn { get; set; }
        // Categories
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        // Common fields
        public string Status { get; set; } = string.Empty;
        public string SmallPhotoUrl { get; set; } = string.Empty;
        public string? LargePhotoUrl { get; set; }
        public DateTime PublishedDate { get; set; }
    }
    public class NewsListDto
    {
        public List<NewsItemDto> News { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class NewsItemDto
    {
        public int Id { get; set; }
        // Titles
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        // Descriptions (truncated for list view)
        public string DescriptionUz { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        // Slugs
        public string? SlugUz { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugEn { get; set; }
        // Meta information (optional for list view)
        public string? MetaTitleUz { get; set; }
        public string? MetaTitleRu { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionUz { get; set; }
        public string? MetaDescriptionRu { get; set; }
        public string? MetaDescriptionEn { get; set; }
        // Categories
        public string? CategoryUz { get; set; }
        public string? CategoryRu { get; set; }
        public string? CategoryEn { get; set; }
        // Media and meta
        public string SmallPhotoUrl { get; set; } = string.Empty;
        public string? LargePhotoUrl { get; set; }
        public DateTime PublishedDate { get; set; }
    }
    public class NewsCategoryDto
    {
        public string Uz { get; set; } = string.Empty;
        public string Ru { get; set; } = string.Empty;
        public string En { get; set; } = string.Empty;
    }
    public class NewsSlugDto
    {
        public int Id { get; set; }
        public string? SlugUz { get; set; }
        public string? SlugRu { get; set; }
        public string? SlugEn { get; set; }
        public string TitleUz { get; set; } = string.Empty;
        public string TitleRu { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
    }
}