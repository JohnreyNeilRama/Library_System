namespace LibrarySystem.Models;

public class UserCategoryBooksViewModel
{
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryDisplayName { get; set; }
    public string? Description { get; set; }
    public List<CategoryBookItem> Books { get; set; } = new List<CategoryBookItem>();
    public string? Search { get; set; }
    public string? Filter { get; set; }
    public int TotalBooksCount { get; set; }
    public int AvailableBooksCount { get; set; }
    public int UnavailableBooksCount { get; set; }
    public int TotalBooks => TotalBooksCount > 0 ? TotalBooksCount : Books?.Count ?? 0;
    public int AvailableCount => TotalBooksCount > 0 ? AvailableBooksCount : Books?.Count(b => b.AvailableCopies > 0) ?? 0;
    public int UnavailableCount => TotalBooksCount > 0 ? UnavailableBooksCount : Books?.Count(b => b.AvailableCopies == 0) ?? 0;
}

public class CategoryBookItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int AvailableCopies { get; set; }
    public string? CoverImageUrl { get; set; }
    public int? ReservationId { get; set; }
    public string? ReservationStatus { get; set; }
}
