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
                BorrowedBooksCount = borrowedBooks.Count(t => t.Status != "Pending" && t.Status != "Approved"),
                ReturnedBooksCount = returnedBooks.Count,
                OverdueBooksCount = borrowedBooks.Count(t => t.Status != "Pending" && t.Status != "Approved" && t.DueDate < DateTime.Now),
                PendingCount = borrowedBooks.Count(t => t.Status == "Pending"),
                ReservedCount = reservations.Count,

                RecentActivity = GetRecentActivity(user.Id),
                CurrentBorrowedBooks = borrowedBooks.Take(3).Select(t => new BorrowedBookItem
                {
                    Id = t.Id,
                    Title = allBooks.FirstOrDefault(b => b.Id == t.BookId)?.Title ?? "Unknown",
                    DueDate = t.DueDate,
                    Status = t.Status == "Pending" ? "Pending Approval" : t.Status == "Approved" ? "Awaiting Claim" : GetStatus(t.DueDate),
                    CanClaim = t.Status == "Approved"
                }).ToList(),
                CurrentReservedBooks = reservations.Take(3).Select(r => new ReservedBookItem
                {
                    Id = r.Id,
                    Title = allBooks.FirstOrDefault(b => b.Id == r.BookId)?.Title ?? "Unknown",
                    Status = r.Status == "Approved" ? "Awaiting Claim" : "Pending Approval",
                    CanClaim = r.Status == "Approved"
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
                string status = t.Status == "Pending" ? "Pending Approval" : t.Status == "Approved" ? "Awaiting Claim" : "On Time";
                if (t.Status != "Pending" && t.Status != "Approved" && t.DueDate < DateTime.Now)
                    status = "Overdue";
                else if (t.Status != "Pending" && t.Status != "Approved" && t.DueDate <= DateTime.Now.AddDays(3))
                    status = "Due Soon";

                return new BorrowedBookItem
                {
                    Id = t.Id,
                    BookId = t.BookId,
                    Title = book?.Title ?? "Unknown",
                    Author = book?.Author ?? "Unknown",
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    Status = status,
                    CanClaim = t.Status == "Approved"
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

            var viewModel = new LibrarySystem.Models.UserCategoriesViewModel
            {
                Categories = categories.Select(c => new LibrarySystem.Models.UserCategoryItem
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

            var categoryKey = category.Trim();
            var cat = _libraryService.GetCategoryById(categoryKey);
            if (cat == null)
                return NotFound();

            var user = GetCurrentUser();
            var reservations = user == null
                ? new List<InMemoryReservation>()
                : _libraryService.GetUserReservations(user.Id);

            var books = _libraryService.GetBooksByCategory(categoryKey)
                .Select(b =>
                {
                    var reservation = reservations.FirstOrDefault(r => r.BookId == b.Id);
                    return new CategoryBookItem
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        ISBN = b.ISBN,
                        AvailableCopies = b.AvailableCopies,
                        CoverImageUrl = b.CoverImageUrl,
                        ReservationId = reservation?.Id,
                        ReservationStatus = reservation?.Status
                    };
                }).ToList();

            return View(new UserCategoryBooksViewModel
            {
                CategoryId = cat.Id,
                CategoryName = cat.Name,
                Books = books
            });
        }

        [HttpPost]
        public IActionResult Reserve(int bookId, string? category)
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var book = _libraryService.GetBookById(bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction("Categories");
            }

            var existingReservation = _libraryService.GetUserReservations(user.Id)
                .FirstOrDefault(r => r.BookId == bookId);

            if (existingReservation != null)
            {
                TempData["ErrorMessage"] = $"You already reserved '{book.Title}'.";
            }
            else if (_libraryService.ReserveBook(user.Id, bookId) != null)
            {
                TempData["SuccessMessage"] = $"Successfully reserved '{book.Title}'.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to reserve '{book.Title}'.";
            }

            var categoryId = !string.IsNullOrWhiteSpace(category)
                ? category
                : _libraryService.GetAllCategories()
                    .FirstOrDefault(c => _libraryService.GetBooksByCategory(c.Id).Any(b => b.Id == bookId))?.Id;

            return RedirectToAction("Books", new { category = categoryId });
        }

        [HttpPost]
        public IActionResult ClaimReservation(int reservationId)
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var reservation = _libraryService.GetUserReservations(user.Id)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction("Dashboard");
            }

            if (reservation.Status != "Approved")
            {
                TempData["ErrorMessage"] = "This reservation is not ready to claim yet.";
                return RedirectToAction("Dashboard");
            }

            if (_libraryService.ClaimReservation(reservationId))
                TempData["SuccessMessage"] = "Reservation claimed successfully.";
            else
                TempData["ErrorMessage"] = "Failed to claim reservation. The claim window may have expired.";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult ClaimBorrowRequest(int transactionId)
        {
            var user = GetCurrentUser();
            if (user == null) return NotFound();

            var transaction = _libraryService.GetUserBorrowedBooks(user.Id)
                .FirstOrDefault(t => t.Id == transactionId);

            if (transaction == null)
            {
                TempData["ErrorMessage"] = "Borrow request not found.";
                return RedirectToAction("Dashboard");
            }

            if (transaction.Status != "Approved")
            {
                TempData["ErrorMessage"] = "This borrow request is not ready to claim yet.";
                return RedirectToAction("Dashboard");
            }

            if (_libraryService.ClaimBorrowRequest(transactionId))
                TempData["SuccessMessage"] = "Borrow request claimed successfully.";
            else
                TempData["ErrorMessage"] = "Failed to claim borrow request. The claim window may have expired.";

            return RedirectToAction("Dashboard");
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
            var today = DateTime.Now.Date;
            var maxEndDate = today.AddDays(14);

            model.BorrowDate = today;

            if (model.EndDate.Date < today)
                ModelState.AddModelError(nameof(model.EndDate), "End date cannot be before the borrow date.");

            if (model.EndDate.Date > maxEndDate)
                ModelState.AddModelError(nameof(model.EndDate), "End date cannot be more than 14 days from today.");

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

            var transaction = _libraryService.BorrowBook(user.Id, model.BookId, model.EndDate);
            var book = _libraryService.GetBookById(model.BookId);

            if (transaction != null)
            {
                if (isAjax)
                    return Json(new { success = true, message = $"Borrow request submitted for '{book?.Title}'. Please wait for admin approval." });

                TempData["SuccessMessage"] = $"Borrow request submitted for '{book?.Title}'. Please wait for admin approval.";
                return RedirectToAction("BorrowedBooks");
            }
            else
            {
                var errorMsg = book?.AvailableCopies <= 0 ? "This book is currently out of stock. Please reserve it instead." : "Book not found or already requested.";

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
        public bool CanClaim { get; set; }
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
        public bool CanClaim { get; set; }
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
