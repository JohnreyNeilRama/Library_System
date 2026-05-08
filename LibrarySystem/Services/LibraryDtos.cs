namespace LibrarySystem.Services
{
    // DTO classes used by the service layer and controllers.
    // These are intentionally separate from the EF Core entity models in Models/Models.cs
    // so that controllers are not tightly coupled to the database schema.

    public class InMemoryUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string MembershipTier { get; set; } = "Standard";
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class InMemoryBook
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public string? CoverImageUrl { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public string CategoryId { get; set; } = string.Empty;
    }

    public class InMemoryCategory
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class InMemoryBorrowTransaction
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "On Time";
    }

    public class InMemoryReservation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
