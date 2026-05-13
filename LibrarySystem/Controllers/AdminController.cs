using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Services;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILibraryService _libraryService;

        public AdminController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var allBooks = _libraryService.GetAllBooks();
            var allUsers = _libraryService.GetAllUsers();
            var allTransactions = _libraryService.GetAllTransactions()
                .OrderByDescending(t => t.ReturnDate ?? t.BorrowDate)
                .ToList();
            var model = new AdminDashboardViewModel
            {
                TotalBooks = allBooks.Count,
                AvailableBooks = allBooks.Sum(b => b.AvailableCopies),
                TotalUsers = allUsers.Count(u => !u.IsAdmin),
                ActiveBorrows = _libraryService.GetActiveBorrows().Count,
                ReturnedBooks = allTransactions.Count(t => t.ReturnDate.HasValue),
                OverdueBooks = _libraryService.GetOverdueBooks().Count,
                TotalCategories = _libraryService.GetAllCategories().Count,
                RecentTransactions = allTransactions.Take(5).ToList(),
                RecentUsers = allUsers.Where(u => !u.IsAdmin).OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
                Books = allBooks,
                Users = allUsers
            };

            return View(model);
        }

        public IActionResult AllBooks(string? search = null, string? category = null)
        {
            var books = _libraryService.GetAllBooks();

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                books = books.Where(b =>
                    b.Title.ToLower().Contains(searchLower) ||
                    b.Author.ToLower().Contains(searchLower) ||
                    b.ISBN.ToLower().Contains(searchLower)).ToList();
            }

            if (!string.IsNullOrEmpty(category))
                books = books.Where(b => b.CategoryId == category).ToList();

            var model = new AdminAllBooksViewModel
            {
                Books = books,
                Categories = _libraryService.GetAllCategories(),
                Search = search,
                SelectedCategory = category
            };

            return View(model);
        }

        public IActionResult ManageBooks()
        {
            return RedirectToAction("Requests");
        }

        public IActionResult Requests()
        {
            var model = new AdminManageBooksViewModel
            {
                Books = _libraryService.GetAllBooks(),
                Categories = _libraryService.GetAllCategories(),
                Transactions = _libraryService.GetActiveBorrows()
                    .OrderBy(t => t.DueDate)
                    .ToList(),
                Users = _libraryService.GetAllUsers(),
                Reservations = _libraryService.GetAllReservations(),
                BorrowRequests = _libraryService.GetAllTransactions()
                    .Where(t => t.Status == "Pending")
                    .OrderByDescending(t => t.BorrowDate)
                    .ToList(),
                ApprovedBorrowRequests = _libraryService.GetAllTransactions()
                    .Where(t => t.Status == "Approved")
                    .OrderByDescending(t => t.BorrowDate)
                    .ToList()
            };

            return View("ManageBooks", model);
        }

        public IActionResult AddBook()
        {
            var model = new AdminBookViewModel
            {
                Categories = _libraryService.GetAllCategories(),
                AvailableCopies = 1,
                TotalCopies = 1
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddBook(AdminBookViewModel model)
        {
            model.Categories = _libraryService.GetAllCategories();

            if (!model.Categories.Any())
                ModelState.AddModelError(nameof(model.CategoryId), "Add a category before adding books.");

            if (model.TotalCopies <= 0)
                model.TotalCopies = model.AvailableCopies;

            if (model.AvailableCopies > model.TotalCopies)
                model.TotalCopies = model.AvailableCopies;

            if (ModelState.IsValid)
            {
                var book = new InMemoryBook
                {
                    Title = model.Title.Trim(),
                    Author = model.Author.Trim(),
                    Description = model.Description?.Trim() ?? string.Empty,
                    ISBN = model.ISBN?.Trim() ?? string.Empty,
                    PublishedYear = model.PublishedYear,
                    CoverImageUrl = string.IsNullOrWhiteSpace(model.CoverImageUrl) ? null : model.CoverImageUrl.Trim(),
                    AvailableCopies = model.AvailableCopies,
                    TotalCopies = model.TotalCopies,
                    CategoryId = model.CategoryId
                };

                _libraryService.AddBook(book);
                TempData["SuccessMessage"] = "Book added successfully!";
                return RedirectToAction("AllBooks");
            }

            return View(model);
        }

        public IActionResult EditBook(int id)
        {
            var book = _libraryService.GetBookById(id);
            if (book == null)
                return NotFound();

            var model = new AdminBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CoverImageUrl = book.CoverImageUrl,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                CategoryId = book.CategoryId,
                Categories = _libraryService.GetAllCategories()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditBook(AdminBookViewModel model)
        {
            model.Categories = _libraryService.GetAllCategories();

            if (ModelState.IsValid)
            {
                var book = new InMemoryBook
                {
                    Id = model.Id,
                    Title = model.Title.Trim(),
                    Author = model.Author.Trim(),
                    Description = model.Description?.Trim() ?? string.Empty,
                    ISBN = model.ISBN?.Trim() ?? string.Empty,
                    PublishedYear = model.PublishedYear,
                    CoverImageUrl = string.IsNullOrWhiteSpace(model.CoverImageUrl) ? null : model.CoverImageUrl.Trim(),
                    AvailableCopies = model.AvailableCopies,
                    TotalCopies = model.TotalCopies,
                    CategoryId = model.CategoryId
                };

                if (_libraryService.UpdateBook(book))
                {
                    TempData["SuccessMessage"] = "Book updated successfully!";
                    return RedirectToAction("AllBooks");
                }

                TempData["ErrorMessage"] = "Failed to update book.";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            if (_libraryService.DeleteBook(id))
                TempData["SuccessMessage"] = "Book deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete book.";

            return RedirectToAction("AllBooks");
        }

        [HttpPost]
        public IActionResult ReturnBook(int transactionId, string? returnTo = null)
        {
            if (_libraryService.ReturnBook(transactionId))
                TempData["SuccessMessage"] = "Book returned successfully!";
            else
                TempData["ErrorMessage"] = "Failed to return book.";

            if (returnTo == "TransactionHistory")
                return RedirectToAction("TransactionHistory");

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult ApproveBorrowRequest(int transactionId)
        {
            if (_libraryService.ApproveBorrowRequest(transactionId))
                TempData["SuccessMessage"] = "Borrow request approved.";
            else
                TempData["ErrorMessage"] = "Failed to approve borrow request. The book may be out of stock.";

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult DeclineBorrowRequest(int transactionId)
        {
            if (_libraryService.DeclineBorrowRequest(transactionId))
                TempData["SuccessMessage"] = "Borrow request declined.";
            else
                TempData["ErrorMessage"] = "Failed to decline borrow request.";

            return RedirectToAction("Requests");
        }

        public IActionResult TransactionHistory(string? filter = null, string? search = null)
        {
            var transactions = _libraryService.GetAllTransactions();

            if (filter == "Borrowed")
                transactions = transactions.Where(t => !t.ReturnDate.HasValue).ToList();
            else if (filter == "Returned")
                transactions = transactions.Where(t => t.ReturnDate.HasValue).ToList();
            else if (filter == "Overdue")
                transactions = _libraryService.GetOverdueBooks();

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                var books = _libraryService.GetAllBooks();
                var users = _libraryService.GetAllUsers();
                transactions = transactions.Where(t =>
                {
                    var book = books.FirstOrDefault(b => b.Id == t.BookId);
                    var user = users.FirstOrDefault(u => u.Id == t.UserId);
                    return (book != null && (book.Title.ToLower().Contains(searchLower) || book.Author.ToLower().Contains(searchLower))) ||
                           (user != null && user.FullName.ToLower().Contains(searchLower));
                }).ToList();
            }

            var model = new AdminTransactionHistoryViewModel
            {
                Transactions = transactions,
                ActiveBorrows = _libraryService.GetActiveBorrows()
                    .OrderBy(t => t.DueDate)
                    .ToList(),
                ReturnedTransactions = _libraryService.GetAllTransactions()
                    .Where(t => t.ReturnDate.HasValue)
                    .OrderByDescending(t => t.ReturnDate)
                    .ToList(),
                Books = _libraryService.GetAllBooks(),
                Users = _libraryService.GetAllUsers(),
                Filter = filter,
                Search = search
            };

            return View(model);
        }

        public IActionResult Categories()
        {
            var model = new AdminCategoriesViewModel
            {
                Categories = _libraryService.GetAllCategories(),
                Books = _libraryService.GetAllBooks()
            };

            return View(model);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(AdminCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new InMemoryCategory
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description
                };

                if (_libraryService.AddCategory(category))
                {
                    TempData["SuccessMessage"] = "Category added successfully!";
                    return RedirectToAction("Categories");
                }

                ModelState.AddModelError(string.Empty, "Category ID already exists.");
            }

            return View(model);
        }

        public IActionResult EditCategory(string id)
        {
            var category = _libraryService.GetCategoryById(id);
            if (category == null)
                return NotFound();

            var model = new AdminCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(AdminCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new InMemoryCategory
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description
                };

                _libraryService.UpdateCategory(category);
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Categories");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteCategory(string id)
        {
            if (_libraryService.DeleteCategory(id))
                TempData["SuccessMessage"] = "Category deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete category. Remove or move its books first.";

            return RedirectToAction("Categories");
        }

        public IActionResult ManageUsers(string? search = null)
        {
            var users = _libraryService.GetAllUsers().Where(u => !u.IsAdmin).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                users = users.Where(u =>
                    u.FullName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    u.UserName.ToLower().Contains(searchLower)).ToList();
            }

            var model = new AdminManageUsersViewModel
            {
                Users = users,
                Search = search
            };

            return View(model);
        }

        public IActionResult EditUser(string id)
        {
            var user = _libraryService.GetUserById(id);
            if (user == null)
                return NotFound();

            var model = new AdminUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                IdNumber = user.IdNumber,
                Course = user.Course,
                YearLevel = user.YearLevel,
                MembershipTier = user.MembershipTier,
                IsActive = user.IsActive
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _libraryService.GetUserById(model.Id);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.IdNumber = model.IdNumber;
                    user.Course = model.Course;
                    user.YearLevel = model.YearLevel;
                    user.MembershipTier = model.MembershipTier;
                    user.IsActive = model.IsActive;

                    _libraryService.UpdateUser(user);
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction("ManageUsers");
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ToggleUserStatus(string id)
        {
            if (_libraryService.ToggleUserActiveStatus(id))
                TempData["SuccessMessage"] = "User status updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to update user status.";

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public IActionResult DeleteUser(string id)
        {
            if (_libraryService.DeleteUser(id))
                TempData["SuccessMessage"] = "User deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete user. Make sure the user has no active borrows or reservations.";

            return RedirectToAction("ManageUsers");
        }

        public IActionResult Reservations()
        {
            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult ApproveReservation(int reservationId)
        {
            if (_libraryService.ApproveReservation(reservationId))
                TempData["SuccessMessage"] = "Reservation approved successfully.";
            else
                TempData["ErrorMessage"] = "Failed to approve reservation.";

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult DeclineReservation(int reservationId)
        {
            if (_libraryService.DeclineReservation(reservationId))
                TempData["SuccessMessage"] = "Reservation declined.";
            else
                TempData["ErrorMessage"] = "Failed to decline reservation.";

            return RedirectToAction("Requests");
        }
    }

        public class AdminDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveBorrows { get; set; }
        public int ReturnedBooks { get; set; }
        public int OverdueBooks { get; set; }
        public int TotalCategories { get; set; }
        public List<InMemoryBorrowTransaction> RecentTransactions { get; set; } = new();
        public List<InMemoryUser> RecentUsers { get; set; } = new();
        public List<InMemoryBook> Books { get; set; } = new();
        public List<InMemoryUser> Users { get; set; } = new();
    }

    public class AdminAllBooksViewModel
    {
        public List<InMemoryBook> Books { get; set; } = new();
        public List<InMemoryCategory> Categories { get; set; } = new();
        public string? Search { get; set; }
        public string? SelectedCategory { get; set; }
    }

    public class AdminManageBooksViewModel
    {
        public List<InMemoryBook> Books { get; set; } = new();
        public List<InMemoryCategory> Categories { get; set; } = new();
        public List<InMemoryBorrowTransaction> Transactions { get; set; } = new();
        public List<InMemoryBorrowTransaction> BorrowRequests { get; set; } = new();
        public List<InMemoryBorrowTransaction> ApprovedBorrowRequests { get; set; } = new();
        public List<InMemoryUser> Users { get; set; } = new();
        public List<InMemoryReservation> Reservations { get; set; } = new();
    }

    public class AdminBookViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ISBN { get; set; }

        [Range(0, 2100)]
        public int PublishedYear { get; set; }

        public string? CoverImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableCopies { get; set; }

        [Range(1, int.MaxValue)]
        public int TotalCopies { get; set; }

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        public List<InMemoryCategory> Categories { get; set; } = new();
    }

    public class AdminTransactionHistoryViewModel
    {
        public List<InMemoryBorrowTransaction> Transactions { get; set; } = new();
        public List<InMemoryBorrowTransaction> ActiveBorrows { get; set; } = new();
        public List<InMemoryBorrowTransaction> ReturnedTransactions { get; set; } = new();
        public List<InMemoryBook> Books { get; set; } = new();
        public List<InMemoryUser> Users { get; set; } = new();
        public string? Filter { get; set; }
        public string? Search { get; set; }
    }

    public class AdminCategoriesViewModel
    {
        public List<InMemoryCategory> Categories { get; set; } = new();
        public List<InMemoryBook> Books { get; set; } = new();
    }

    public class AdminCategoryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AdminManageUsersViewModel
    {
        public List<InMemoryUser> Users { get; set; } = new();
        public string? Search { get; set; }
    }

    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string MembershipTier { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AdminReservationsViewModel
    {
        public List<InMemoryReservation> Reservations { get; set; } = new();
        public List<InMemoryBook> Books { get; set; } = new();
        public List<InMemoryUser> Users { get; set; } = new();
    }
}
