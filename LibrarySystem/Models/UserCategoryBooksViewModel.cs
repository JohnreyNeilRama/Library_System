namespace LibrarySystem.Models;

public class UserCategoryBooksViewModel
{
    public string? CategoryName { get; set; }
    public string? CategoryDisplayName { get; set; }
    public string? Description { get; set; }
    public List<CategoryBookItem> Books { get; set; } = new List<CategoryBookItem>();
    public string? Search { get; set; }
    public string? Filter { get; set; }
}

public class CategoryBookItem
{
    public int Id { get; set; }
}
