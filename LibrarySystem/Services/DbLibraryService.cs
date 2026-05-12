/**
 * DbLibraryService.cs
 * 
 * Implementation of ILibraryService using Entity Framework Core and SQL Server.
 * This class handles all database interactions for the application.
 * 
 * This is the updated version of the file.
 */

using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;

// This file replaces LibraryService.cs and InMemoryDataStore.cs
// It implements the same ILibraryService interface your controllers already use,
// so your frontend code does NOT need to change at all.

namespace LibrarySystem.Services
{
    /**
     * Service class for handling database-backed library operations.
     */
    public class DbLibraryService : ILibraryService
    {
        private readonly AppDbContext _db;

        public DbLibraryService(AppDbContext db)
        {
            _db = db;
        }

        // ── USERS ────────────────────────────────────────────────────────────

        public InMemoryUser? GetUserById(string userId)
        {
            var u = _db.Users.FirstOrDefault(x => x.Id == userId);
            return u == null ? null : MapUser(u);
        }

        public InMemoryUser? GetUserByEmail(string email)
        {
            var u = _db.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            return u == null ? null : MapUser(u);
        }

        public InMemoryUser? GetUserByUserName(string userName)
        {
            var u = _db.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower());
            return u == null ? null : MapUser(u);
        }

        public List<InMemoryUser> GetAllUsers()
        {
            return _db.Users.AsNoTracking().AsEnumerable().Select(u => MapUser(u)).ToList();
        }

        public bool CreateUser(InMemoryUser user)
        {
            if (_db.Users.Any(u => u.Email == user.Email || u.UserName == user.UserName))
                return false;

            var newUser = new User
            {
                Id = user.Id, // Use the ID from the InMemoryUser object
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                Course = user.Course,
                YearLevel = user.YearLevel,
                MembershipTier = user.MembershipTier,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsVerified = user.IsVerified,
                IsAdmin = user.IsAdmin,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();
            return true;
        }

        public bool ValidateUser(string userNameOrEmail, string password)
        {
            var user = _db.Users.FirstOrDefault(u =>
                u.UserName.ToLower() == userNameOrEmail.ToLower() ||
                u.Email.ToLower() == userNameOrEmail.ToLower());

            if (user == null || !user.IsActive) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public bool UpdateUser(InMemoryUser user)
        {
            var existing = _db.Users.Find(user.Id);
            if (existing == null) return false;

            existing.FullName = user.FullName;
            existing.IdNumber = user.IdNumber;
            existing.Course = user.Course;
            existing.YearLevel = user.YearLevel;
            existing.Email = user.Email;
            existing.ProfilePictureUrl = user.ProfilePictureUrl;
            existing.MembershipTier = user.MembershipTier;
            existing.IsAdmin = user.IsAdmin;
            _db.SaveChanges();
            return true;
        }

        public bool ToggleUserActiveStatus(string userId)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            _db.SaveChanges();
            return true;
        }

        // ── BOOKS ─────────────────────────────────────────────────────────────

        public InMemoryBook? GetBookById(int bookId)
        {
            var b = _db.Books.Include(x => x.Category).FirstOrDefault(x => x.Id == bookId);
            return b == null ? null : MapBook(b);
        }

        public List<InMemoryBook> GetAllBooks()
        {
            return _db.Books.AsNoTracking().Include(b => b.Category)
                .Select(b => MapBook(b)).ToList();
        }

        public List<InMemoryBook> GetBooksByCategory(string categoryId)
        {
            return _db.Books.AsNoTracking().Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .Select(b => MapBook(b)).ToList();
        }

        public bool AddBook(InMemoryBook book)
        {
            _db.Books.Add(new Book
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CoverImageUrl = book.CoverImageUrl,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                CategoryId = book.CategoryId
            });
            _db.SaveChanges();
            return true;
        }

        public bool UpdateBook(InMemoryBook book)
        {
            var existing = _db.Books.Find(book.Id);
            if (existing == null) return false;

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Description = book.Description;
            existing.ISBN = book.ISBN;
            existing.PublishedYear = book.PublishedYear;
            existing.CoverImageUrl = book.CoverImageUrl;
            existing.AvailableCopies = book.AvailableCopies;
            existing.TotalCopies = book.TotalCopies;
            existing.CategoryId = book.CategoryId;
            _db.SaveChanges();
            return true;
        }

        public bool DeleteBook(int bookId)
        {
            var book = _db.Books.Find(bookId);
            if (book == null) return false;

            _db.Books.Remove(book);
            _db.SaveChanges();
            return true;
        }

        // ── CATEGORIES ────────────────────────────────────────────────────────

        public List<InMemoryCategory> GetAllCategories()
        {
            return _db.Categories.AsNoTracking()
                .Select(c => new InMemoryCategory { Id = c.Id, Name = c.Name, Description = c.Description })
                .ToList();
        }

        public InMemoryCategory? GetCategoryById(string categoryId)
        {
            var c = _db.Categories.Find(categoryId);
            return c == null ? null : new InMemoryCategory { Id = c.Id, Name = c.Name, Description = c.Description };
        }

        public bool AddCategory(InMemoryCategory category)
        {
            if (_db.Categories.Any(c => c.Id == category.Id)) return false;

            _db.Categories.Add(new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
            _db.SaveChanges();
            return true;
        }

        public bool UpdateCategory(InMemoryCategory category)
        {
            var existing = _db.Categories.Find(category.Id);
            if (existing == null) return false;

            existing.Name = category.Name;
            existing.Description = category.Description;
            _db.SaveChanges();
            return true;
        }

        public bool DeleteCategory(string categoryId)
        {
            var category = _db.Categories.Find(categoryId);
            if (category == null) return false;
            if (_db.Books.Any(b => b.CategoryId == categoryId)) return false;

            _db.Categories.Remove(category);
            _db.SaveChanges();
            return true;
        }

        // ── BORROW TRANSACTIONS ───────────────────────────────────────────────

        public InMemoryBorrowTransaction? BorrowBook(string userId, int bookId, DateTime? requestedEndDate = null, int borrowDays = 14)
        {
            var book = _db.Books.Find(bookId);
            var user = _db.Users.Find(userId);
            if (book == null || user == null || book.AvailableCopies <= 0) return null;

            var hasOpenRequest = _db.BorrowTransactions.Any(t =>
                t.UserId == userId &&
                t.BookId == bookId &&
                t.ReturnDate == null &&
                (t.Status == "Pending" || t.Status == "On Time"));

            if (hasOpenRequest) return null;

            var transaction = new BorrowTransaction
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = (requestedEndDate ?? DateTime.Now.AddDays(borrowDays)).Date.AddDays(1).AddTicks(-1),
                Status = "Pending"
            };

            _db.BorrowTransactions.Add(transaction);
            _db.SaveChanges();
            return MapTransaction(transaction);
        }

        public bool ApproveBorrowRequest(int transactionId, int borrowDays = 14)
        {
            var t = _db.BorrowTransactions.Find(transactionId);
            if (t == null || t.ReturnDate.HasValue || t.Status != "Pending") return false;

            var book = _db.Books.Find(t.BookId);
            if (book == null || book.AvailableCopies <= 0) return false;

            book.AvailableCopies--;
            t.BorrowDate = DateTime.Now;
            t.Status = "Approved";

            _db.SaveChanges();
            return true;
        }

        public bool DeclineBorrowRequest(int transactionId)
        {
            var t = _db.BorrowTransactions.Find(transactionId);
            if (t == null || t.ReturnDate.HasValue || t.Status != "Pending") return false;

            t.Status = "Declined";
            _db.SaveChanges();
            return true;
        }

        public bool ClaimBorrowRequest(int transactionId, int borrowDays = 14)
        {
            var t = _db.BorrowTransactions.Find(transactionId);
            if (t == null || t.ReturnDate.HasValue || t.Status != "Approved") return false;
            if (t.BorrowDate.AddHours(24) < DateTime.Now) return false;

            t.BorrowDate = DateTime.Now;
            t.Status = "On Time";

            _db.SaveChanges();
            return true;
        }

        public bool ReturnBook(int transactionId)
        {
            var t = _db.BorrowTransactions.Find(transactionId);
            if (t == null || t.ReturnDate.HasValue || t.Status == "Pending" || t.Status == "Approved" || t.Status == "Declined") return false;

            t.ReturnDate = DateTime.Now;
            t.Status = t.ReturnDate.Value <= t.DueDate ? "On Time" : "Returned Late";

            var book = _db.Books.Find(t.BookId);
            if (book != null) book.AvailableCopies++;

            _db.SaveChanges();
            return true;
        }

        public List<InMemoryBorrowTransaction> GetUserBorrowedBooks(string userId)
        {
            return _db.BorrowTransactions
                .Where(t => t.UserId == userId && t.ReturnDate == null && t.Status != "Declined")
                .Select(t => MapTransaction(t)).ToList();
        }

        public List<InMemoryBorrowTransaction> GetUserReturnedBooks(string userId)
        {
            return _db.BorrowTransactions
                .Where(t => t.UserId == userId && t.ReturnDate != null && t.Status != "Declined")
                .Select(t => MapTransaction(t)).ToList();
        }

        public List<InMemoryBorrowTransaction> GetAllTransactions()
        {
            return _db.BorrowTransactions.AsNoTracking()
                .Select(t => MapTransaction(t)).ToList();
        }

        public List<InMemoryBorrowTransaction> GetActiveBorrows()
        {
            return _db.BorrowTransactions
                .Where(t => t.ReturnDate == null && t.Status != "Pending" && t.Status != "Approved" && t.Status != "Declined")
                .Select(t => MapTransaction(t)).ToList();
        }

        public List<InMemoryBorrowTransaction> GetOverdueBooks()
        {
            var now = DateTime.Now;
            return _db.BorrowTransactions
                .Where(t => t.ReturnDate == null && t.Status != "Pending" && t.Status != "Approved" && t.Status != "Declined" && t.DueDate < now)
                .Select(t => MapTransaction(t)).ToList();
        }

        // ── RESERVATIONS ──────────────────────────────────────────────────────

        public InMemoryReservation? ReserveBook(string userId, int bookId, int reserveDays = 7)
        {
            var book = _db.Books.Find(bookId);
            var user = _db.Users.Find(userId);
            if (book == null || user == null || book.AvailableCopies > 0) return null;

            var reservation = new Reservation
            {
                UserId = userId,
                BookId = bookId,
                ReservationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(reserveDays),
                Status = "Pending",
                IsActive = true
            };

            _db.Reservations.Add(reservation);
            _db.SaveChanges();
            return MapReservation(reservation);
        }

        public bool ApproveReservation(int reservationId, int claimHours = 24)
        {
            var r = _db.Reservations.Find(reservationId);
            if (r == null || !r.IsActive || r.Status != "Pending") return false;

            var now = DateTime.Now;
            r.Status = "Approved";
            r.ApprovedAt = now;
            r.ClaimDeadline = now.AddHours(claimHours);
            _db.SaveChanges();
            return true;
        }

        public bool DeclineReservation(int reservationId)
        {
            var r = _db.Reservations.Find(reservationId);
            if (r == null || !r.IsActive || r.Status != "Pending") return false;

            r.Status = "Declined";
            r.IsActive = false;
            _db.SaveChanges();
            return true;
        }

        public bool ClaimReservation(int reservationId)
        {
            var r = _db.Reservations.Find(reservationId);
            if (r == null || !r.IsActive || r.Status != "Approved") return false;
            if (r.ClaimDeadline.HasValue && r.ClaimDeadline.Value < DateTime.Now) return false;
            var book = _db.Books.Find(r.BookId);
            if (_db.Users.Find(r.UserId) == null || book == null) return false;

            if (book.AvailableCopies > 0)
                book.AvailableCopies--;
            r.Status = "Claimed";
            r.ClaimedAt = DateTime.Now;
            r.IsActive = false;
            _db.BorrowTransactions.Add(new BorrowTransaction
            {
                UserId = r.UserId,
                BookId = r.BookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "On Time"
            });
            _db.SaveChanges();
            return true;
        }

        public bool CancelReservation(int reservationId)
        {
            var r = _db.Reservations.Find(reservationId);
            if (r == null || !r.IsActive) return false;

            r.IsActive = false;
            _db.SaveChanges();
            return true;
        }

        public List<InMemoryReservation> GetUserReservations(string userId)
        {
            return _db.Reservations
                .Where(r => r.UserId == userId && r.IsActive && r.Status != "Declined" && r.Status != "Expired")
                .Select(r => MapReservation(r)).ToList();
        }

        public List<InMemoryReservation> GetAllReservations()
        {
            return _db.Reservations.AsNoTracking()
                .Select(r => MapReservation(r)).ToList();
        }

        // ── MAPPERS (DB model → InMemory model your controllers expect) ───────

        private static InMemoryUser MapUser(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            PasswordHash = u.PasswordHash,
            FullName = u.FullName,
            IdNumber = u.IdNumber,
            Course = u.Course,
            YearLevel = u.YearLevel,
            MembershipTier = u.MembershipTier,
            ProfilePictureUrl = u.ProfilePictureUrl,
            IsVerified = u.IsVerified,
            IsAdmin = u.IsAdmin,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };

        private static InMemoryBook MapBook(Book b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Description = b.Description,
            ISBN = b.ISBN,
            PublishedYear = b.PublishedYear,
            CoverImageUrl = b.CoverImageUrl,
            AvailableCopies = b.AvailableCopies,
            TotalCopies = b.TotalCopies,
            CategoryId = b.CategoryId
        };

        private static InMemoryBorrowTransaction MapTransaction(BorrowTransaction t) => new()
        {
            Id = t.Id,
            UserId = t.UserId,
            BookId = t.BookId,
            BorrowDate = t.BorrowDate,
            DueDate = t.DueDate,
            ReturnDate = t.ReturnDate,
            Status = t.Status
        };

        private static InMemoryReservation MapReservation(Reservation r) => new()
        {
            Id = r.Id,
            UserId = r.UserId,
            BookId = r.BookId,
            ReservationDate = r.ReservationDate,
            ExpiryDate = r.ExpiryDate,
            Status = r.Status,
            ApprovedAt = r.ApprovedAt,
            ClaimDeadline = r.ClaimDeadline,
            ClaimedAt = r.ClaimedAt,
            IsActive = r.IsActive
        };
    }
}
