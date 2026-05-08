using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    // ─── USER ────────────────────────────────────────────────────────────────
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string IdNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Course { get; set; } = string.Empty;

        [MaxLength(20)]
        public string YearLevel { get; set; } = string.Empty;

        [MaxLength(30)]
        public string MembershipTier { get; set; } = "Standard";

        public string ProfilePictureUrl { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    // ─── CATEGORY ────────────────────────────────────────────────────────────
    public class Category
    {
        [Key, MaxLength(60)]
        public string Id { get; set; } = string.Empty;      // e.g. "programming"

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Navigation
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    // ─── BOOK ─────────────────────────────────────────────────────────────────
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [MaxLength(30)]
        public string ISBN { get; set; } = string.Empty;

        public int PublishedYear { get; set; }

        public string? CoverImageUrl { get; set; }

        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }

        // FK → Category
        [MaxLength(60)]
        public string CategoryId { get; set; } = string.Empty;
        public Category? Category { get; set; }

        // Navigation
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    // ─── BORROW TRANSACTION ───────────────────────────────────────────────────
    public class BorrowTransaction
    {
        public int Id { get; set; }

        // FK → User
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        // FK → Book
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "On Time"; // "On Time" | "Returned Late"
    }

    // ─── RESERVATION ─────────────────────────────────────────────────────────
    public class Reservation
    {
        public int Id { get; set; }

        // FK → User
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        // FK → Book
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public DateTime ReservationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
