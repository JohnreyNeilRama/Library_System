/**
 * UserController.cs
 * 
 * Provides user-facing functionality including dashboard,
 * profile management, book borrowing, and transaction history.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using LibrarySystem.Models;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IWebHostEnvironment _environment;

        public UserController(ILibraryService libraryService, IWebHostEnvironment environment)
        {
            _libraryService = libraryService;
            _environment = environment;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        }

        private InMemoryUser? GetCurrentUser()
        {
            return _libraryService.GetUserById(GetCurrentUserId());
        }

        /**
         * GET: /User/Dashboard
         * Displays the user's dashboard with their stats, recent activity, and currently borrowed books.
         */
        public IActionResult Dashboard()
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var borrowedBooks = _libraryService.GetUserBorrowedBooks(user.Id);
            var returnedBooks = _libraryService.GetUserReturnedBooks(user.Id);
            var reservations = _libraryService.GetUserReservations(user.Id);
            var allBooks = _libraryService.GetAllBooks();

            var userData = new UserInfoViewModel
            {
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                Course = user.Course,
                YearLevel = user.YearLevel,
                Email = user.Email,
                MembershipTier = user.MembershipTier,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsVerified = user.IsVerified,
                BorrowedBooksCount = borrowedBooks.Count,
                ReturnedBooksCount = returnedBooks.Count,
                OverdueBooksCount = borrowedBooks.Count(t => t.DueDate < DateTime.Now),
                PendingCount = borrowedBooks.Count(t => t.Status == "Pending"),
                ReservedCount = reservations.Count,

                RecentActivity = GetRecentActivity(user.Id),
                CurrentBorrowedBooks = borrowedBooks.Take(3).Select(t => new BorrowedBookItem
                {
                    Id = t.Id,
                    Title = allBooks.FirstOrDefault(b => b.Id == t.BookId)?.Title ?? "Unknown",
                    DueDate = t.DueDate,
                    Status = GetStatus(t.DueDate)
                }).ToList(),
                CurrentReservedBooks = reservations.Take(3).Select(r => new ReservedBookItem
                {
                    Id = r.Id,
                    Title = allBooks.FirstOrDefault(b => b.Id == r.BookId)?.Title ?? "Unknown",
                    Status = "Ready for Pickup"
                }).ToList()
            };

            return View(userData);
        }

        private string GetStatus(DateTime dueDate)
        {
            if (dueDate < DateTime.Now) return "Overdue";
            if (dueDate <= DateTime.Now.AddDays(3)) return "Due Soon";
            return "On Time";
        }

        private List<ActivityItem> GetRecentActivity(string userId)
        {
            var activity = new List<ActivityItem>();
            var borrows = _libraryService.GetUserBorrowedBooks(userId);
            var returns = _libraryService.GetUserReturnedBooks(userId);
            var reservations = _libraryService.GetUserReservations(userId);
            var books = _libraryService.GetAllBooks();

            foreach (var b in borrows)
                activity.Add(new ActivityItem
                {
                    Icon = "📖",
                    Text = $"Borrowed \"{books.FirstOrDefault(x => x.Id == b.BookId)?.Title}\"",
                    Date = b.BorrowDate
                });

            foreach (var r in returns)
                activity.Add(new ActivityItem
                {
                    Icon = "📗",
                    Text = $"Returned \"{books.FirstOrDefault(x => x.Id == r.BookId)?.Title}\"",
                    Date = r.ReturnDate ?? DateTime.Now
                });

            foreach (var res in reservations)
                activity.Add(new ActivityItem
                {
                    Icon = "📚",
                    Text = $"Reserved \"{books.FirstOrDefault(x => x.Id == res.BookId)?.Title}\"",
                    Date = res.ReservationDate
                });

            return activity.OrderByDescending(a => a.Date).Take(5).ToList();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                Course = user.Course,
                YearLevel = user.YearLevel,
                Email = user.Email,
                ExistingPictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                var updatedUser = new InMemoryUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    PasswordHash = user.PasswordHash,
                    IsAdmin = user.IsAdmin,
                    IsActive = user.IsActive,
                    IsVerified = user.IsVerified,
                    MembershipTier = user.MembershipTier,
                    CreatedAt = user.CreatedAt,

                    FullName = model.FullName,
                    IdNumber = model.IdNumber,
                    Course = model.Course,
                    YearLevel = model.YearLevel,
                    Email = model.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                        Directory.CreateDirectory(uploadsFolder);

                        var extension = Path.GetExtension(model.ProfilePicture.FileName);
                        var uniqueFileName = $"{updatedUser.Id}_{DateTime.Now.Ticks}{extension}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        await model.ProfilePicture.CopyToAsync(fileStream);

                        updatedUser.ProfilePictureUrl = "/uploads/profiles/" + uniqueFileName;
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Error saving the profile picture. Please try again.");
                        model.ExistingPictureUrl = user.ProfilePictureUrl;
                        return View(model);
                    }
                }

                if (_libraryService.UpdateUser(updatedUser))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, updatedUser.Id),
                        new Claim(ClaimTypes.Name, updatedUser.UserName),
                        new Claim(ClaimTypes.Email, updatedUser.Email),
                        new Claim(ClaimTypes.GivenName, updatedUser.FullName),
                        new Claim("IsAdmin", updatedUser.IsAdmin.ToString())
                    };

                    if (updatedUser.IsAdmin)
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile in the database.";
                }

                return RedirectToAction("Profile");
            }

            model.ExistingPictureUrl = user.ProfilePictureUrl;
            return View(model);
        }

        public IActionResult BorrowedBooks()
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var transactions = _libraryService.GetUserBorrowedBooks(user.Id);
            var books = _libraryService.GetAllBooks();

            var borrowedBooks = transactions.Select(t =>
            {
                var book = books.FirstOrDefault(b => b.Id == t.BookId);
                string status = "On Time";
                if (t.DueDate < DateTime.Now)
                    status = "Overdue";
                else if (t.DueDate <= DateTime.Now.AddDays(3))
                    status = "Due Soon";

                return new BorrowedBookItem
                {
                    Id = t.Id,
                    BookId = t.BookId,
                    Title = book?.Title ?? "Unknown",
                    Author = book?.Author ?? "Unknown",
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    Status = status
                };
            }).ToList();

            return View(new UserBorrowedBooksViewModel { BorrowedBooks = borrowedBooks });
        }

        public IActionResult ReturnedBooks()
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var transactions = _libraryService.GetUserReturnedBooks(user.Id);
            var books = _libraryService.GetAllBooks();

            var returnedBooks = transactions.Select(t =>
            {
                var book = books.FirstOrDefault(b => b.Id == t.BookId);
                return new ReturnedBookItem
                {
                    Title = book?.Title ?? "Unknown",
                    Author = book?.Author ?? "Unknown",
                    BorrowDate = t.BorrowDate,
                    ReturnDate = t.ReturnDate!.Value,
                    Status = t.Status
                };
            }).ToList();

            return View(new UserReturnedBooksViewModel { ReturnedBooks = returnedBooks });
        }

        public IActionResult Categories()
        {
            var categories = _libraryService.GetAllCategories();
            var books = _libraryService.GetAllBooks();

            var viewModel = new UserCategoriesViewModel
            {
                Categories = categories.Select(c => new UserCategoryItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    BookCount = books.Count(b => b.CategoryId == c.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult TransactionHistory(string filter = "All", string search = "")
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var transactions = _libraryService.GetAllTransactions()
                .Where(t => t.UserId == user.Id)
                .ToList();

            var books = _libraryService.GetAllBooks();

            var transactionItems = transactions.Select(t =>
            {
                var book = books.FirstOrDefault(b => b.Id == t.BookId);
                return new TransactionItem
                {
                    Id = t.Id,
                    Title = book?.Title ?? "Unknown",
                    Author = book?.Author ?? "Unknown",
                    BorrowDate = t.BorrowDate,
                    ReturnDate = t.ReturnDate,
                    Status = t.Status
                };
            })
            .OrderByDescending(t => t.BorrowDate)
            .ToList();

            if (filter == "Borrowed")
                transactionItems = transactionItems.Where(t => !t.ReturnDate.HasValue).ToList();
            else if (filter == "Returned")
                transactionItems = transactionItems.Where(t => t.ReturnDate.HasValue).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                transactionItems = transactionItems.Where(t =>
                    t.Title.ToLower().Contains(searchLower) ||
                    t.Author.ToLower().Contains(searchLower)).ToList();
            }

            return View(new UserTransactionHistoryViewModel
            {
                Transactions = transactionItems,
                Filter = filter,
                Search = search
            });
        }

        [HttpGet]
        public IActionResult Books(string? category)
        {
            if (string.IsNullOrEmpty(category))
                return RedirectToAction("Categories");

            var categoryKey = category.ToLower();
            var cat = _libraryService.GetCategoryById(categoryKey);
            if (cat == null)
                return NotFound();

            var books = _libraryService.GetBooksByCategory(categoryKey)
                .Select(b => new CategoryBookItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    AvailableCopies = b.AvailableCopies,
                    CoverImageUrl = b.CoverImageUrl
                }).ToList();

            return View(new UserCategoryBooksViewModel
            {
                CategoryName = cat.Name,
                Books = books
            });
        }

        [HttpGet]
        public IActionResult Borrow(int bookId)
        {
            var book = _libraryService.GetBookById(bookId);
            if (book == null)
                return NotFound();

            return View(new BorrowBookViewModel
            {
                BookId = bookId,
                BookTitle = book.Title,
                Author = book.Author
            });
        }

        [HttpPost]
        public IActionResult Borrow(BorrowBookViewModel model)
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    return Json(new { success = false, errors });
                }
                return View(model);
            }

            var transaction = _libraryService.BorrowBook(user.Id, model.BookId);
            var book = _libraryService.GetBookById(model.BookId);

            if (transaction != null)
            {
                if (isAjax)
                    return Json(new { success = true, message = $"Successfully borrowed '{book?.Title}'!" });

                TempData["SuccessMessage"] = $"Successfully borrowed '{book?.Title}'!";
                return RedirectToAction("BorrowedBooks");
            }
            else
            {
                var errorMsg = book?.AvailableCopies <= 0 ? "This book is currently out of stock." : "Book not found.";

                if (isAjax)
                    return Json(new { success = false, message = errorMsg });

                var category = _libraryService.GetAllCategories()
                    .FirstOrDefault(c => _libraryService.GetBooksByCategory(c.Id).Any(b => b.Id == model.BookId))?.Id;

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Books", new { category });
            }
        }
    }

    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IFormFile? ProfilePicture { get; set; }
        public string ExistingPictureUrl { get; set; } = string.Empty;
    }

    public class UserInfoViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MembershipTier { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public int BorrowedBooksCount { get; set; }
        public int ReturnedBooksCount { get; set; }
        public int OverdueBooksCount { get; set; }
        public int PendingCount { get; set; }
        public int ReservedCount { get; set; }

        public List<ActivityItem> RecentActivity { get; set; } = new();
        public List<BorrowedBookItem> CurrentBorrowedBooks { get; set; } = new();
        public List<ReservedBookItem> CurrentReservedBooks { get; set; } = new();
    }

    public class ActivityItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class ReservedBookItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UserBorrowedBooksViewModel
    {
        public List<BorrowedBookItem> BorrowedBooks { get; set; } = new();
    }

    public class BorrowedBookItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserReturnedBooksViewModel
    {
        public List<ReturnedBookItem> ReturnedBooks { get; set; } = new();
    }

    public class ReturnedBookItem
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserTransactionHistoryViewModel
    {
        public List<TransactionItem> Transactions { get; set; } = new();
        public string Filter { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
    }

    public class TransactionItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

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
    }
}
