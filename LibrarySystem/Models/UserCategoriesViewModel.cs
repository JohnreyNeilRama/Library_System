namespace LibrarySystem.Models
{
    public class UserCategoriesViewModel
    {
        public List<UserCategoryItem> Categories { get; set; } = new();
    }

    public class UserCategoryItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BookCount { get; set; }
        public string Initials
        {
            get
            {
                var source = string.IsNullOrWhiteSpace(Name) ? Id : Name;
                var words = source.Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries);

                if (words.Length == 0)
                    return "?";

                return string.Concat(words.Take(2).Select(w => char.ToUpperInvariant(w[0])));
            }
        }
    }
}
