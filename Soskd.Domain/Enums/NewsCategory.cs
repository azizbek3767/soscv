// Soskd.Domain/Enums/NewsCategory.cs
namespace Soskd.Domain.Enums
{
    public enum NewsCategory
    {
        Education = 1,
        Events = 2,
        Memorandums = 3
    }
    public static class NewsCategoryExtensions
    {
        public static (string Uz, string Ru, string En) GetTranslations(this NewsCategory category)
        {
            return category switch
            {
                NewsCategory.Education => ("Ta'lim", "Образование", "Education"),
                NewsCategory.Events => ("Tadbirlar", "События", "Events"),
                NewsCategory.Memorandums => ("Memorandumlar", "Меморандумы", "Memorandums"),
                _ => ("", "", "")
            };
        }
        public static List<(NewsCategory Category, string Uz, string Ru, string En)> GetAllCategories()
        {
            return new List<(NewsCategory, string, string, string)>
            {
                (NewsCategory.Education, "Ta'lim", "Образование", "Education"),
                (NewsCategory.Events, "Tadbirlar", "События", "Events"),
                (NewsCategory.Memorandums, "Memorandumlar", "Меморандумы", "Memorandums")
            };
        }
    }
}