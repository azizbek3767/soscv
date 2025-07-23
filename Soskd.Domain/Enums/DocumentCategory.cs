// Soskd.Domain/Enums/DocumentCategory.cs
namespace Soskd.Domain.Enums
{
    public enum DocumentCategory
    {
        Documents = 1,
        SecurityAndValues = 2
    }
    public static class DocumentCategoryExtensions
    {
        public static (string Uz, string Ru, string En) GetTranslations(this DocumentCategory category)
        {
            return category switch
            {
                DocumentCategory.Documents => ("Hujjatlar", "Документы", "Documents"),
                DocumentCategory.SecurityAndValues => ("Xavfsizlik va qadriyatlar", "Безопасность и ценности", "Security and Values"),
                _ => ("", "", "")
            };
        }
        public static List<(DocumentCategory Category, string Uz, string Ru, string En)> GetAllCategories()
        {
            return Enum.GetValues<DocumentCategory>()
                .Select(c => (c, c.GetTranslations().Uz, c.GetTranslations().Ru, c.GetTranslations().En))
                .ToList();
        }
    }
}