namespace LibrarySystem.Models;

public class UserCategoryBooksViewModel
{
    public string? CategoryName { get; set; }
    public string? CategoryDisplayName { get; set; }
    public string? Description { get; set; }
    public List<CategoryBookItem> Books { get; set; } = new List<CategoryBookItem>();
    public string? Search { get; set; }
    public string? Filter { get; set; }
    public int TotalBooks => Books?.Count ?? 0;
    public int AvailableCount => Books?.Count(b => b.AvailableCopies > 0) ?? 0;
    public int UnavailableCount => Books?.Count(b => b.AvailableCopies == 0) ?? 0;
}

public class CategoryBookItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int AvailableCopies { get; set; }
    public string? CoverImageUrl { get; set; }
}
