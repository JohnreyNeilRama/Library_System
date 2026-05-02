using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            var userData = new UserInfoViewModel {
                FullName = "John Doe",
                IdNumber = "20210001",
                Course = "Computer Science",
                YearLevel = "3rd Year",
                Email = "john.doe@university.edu",
                MembershipTier = "Gold Member",
                ProfilePictureUrl = "https://ui-avatars.com/api/?name=John+Doe&background=0d6efd&color=fff&size=80",
                IsVerified = true
            };
            return View(userData);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var model = new ProfileViewModel {
                FullName = "John Doe",
                IdNumber = "20210001",
                Course = "Computer Science",
                YearLevel = "3rd Year",
                Email = "john.doe@university.edu",
                ExistingPictureUrl = "https://ui-avatars.com/api/?name=John+Doe&background=0d6efd&color=fff&size=150"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid) {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            return View(model);
        }

        public IActionResult BorrowedBooks()
        {
            var model = new UserBorrowedBooksViewModel {
                BorrowedBooks = new List<BorrowedBookItem> {
                    new BorrowedBookItem { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", BorrowDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(4), Status = "On Time" },
                    new BorrowedBookItem { Title = "To Kill a Mockingbird", Author = "Harper Lee", BorrowDate = DateTime.Now.AddDays(-15), DueDate = DateTime.Now.AddDays(-2), Status = "Due Soon" },
                    new BorrowedBookItem { Title = "1984", Author = "George Orwell", BorrowDate = DateTime.Now.AddDays(-20), DueDate = DateTime.Now.AddDays(-5), Status = "Overdue" },
                    new BorrowedBookItem { Title = "Pride and Prejudice", Author = "Jane Austen", BorrowDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(10), Status = "On Time" },
                    new BorrowedBookItem { Title = "The Catcher in the Rye", Author = "J.D. Salinger", BorrowDate = DateTime.Now.AddDays(-8), DueDate = DateTime.Now.AddDays(2), Status = "Due Soon" },
                    new BorrowedBookItem { Title = "Brave New World", Author = "Aldous Huxley", BorrowDate = DateTime.Now.AddDays(-12), DueDate = DateTime.Now.AddDays(-1), Status = "Overdue" }
                }
            };
            return View(model);
        }

        public IActionResult ReturnedBooks()
        {
            var model = new UserReturnedBooksViewModel {
                ReturnedBooks = new List<ReturnedBookItem> {
                    new ReturnedBookItem { Title = "The Alchemist", Author = "Paulo Coelho", BorrowDate = DateTime.Now.AddDays(-30), ReturnDate = DateTime.Now.AddDays(-5), Status = "On Time" },
                    new ReturnedBookItem { Title = "The Hobbit", Author = "J.R.R. Tolkien", BorrowDate = DateTime.Now.AddDays(-25), ReturnDate = DateTime.Now.AddDays(-3), Status = "On Time" },
                    new ReturnedBookItem { Title = "Dune", Author = "Frank Herbert", BorrowDate = DateTime.Now.AddDays(-20), ReturnDate = DateTime.Now.AddDays(-1), Status = "Returned Late" },
                    new ReturnedBookItem { Title = "Foundation", Author = "Isaac Asimov", BorrowDate = DateTime.Now.AddDays(-40), ReturnDate = DateTime.Now.AddDays(-10), Status = "On Time" }
                }
            };
            return View(model);
        }

        public IActionResult Categories()
        {
            return View(new UserCategoriesViewModel());
        }

        public IActionResult TransactionHistory(string filter = "All", string search = "")
        {
            // Get sample data - in real app, this would come from database
            var borrowedBooks = new List<BorrowedBookItem> {
                new BorrowedBookItem { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", BorrowDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(4), Status = "On Time" },
                new BorrowedBookItem { Title = "To Kill a Mockingbird", Author = "Harper Lee", BorrowDate = DateTime.Now.AddDays(-15), DueDate = DateTime.Now.AddDays(-2), Status = "Due Soon" },
                new BorrowedBookItem { Title = "1984", Author = "George Orwell", BorrowDate = DateTime.Now.AddDays(-20), DueDate = DateTime.Now.AddDays(-5), Status = "Overdue" },
                new BorrowedBookItem { Title = "Pride and Prejudice", Author = "Jane Austen", BorrowDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(10), Status = "On Time" },
                new BorrowedBookItem { Title = "The Catcher in the Rye", Author = "J.D. Salinger", BorrowDate = DateTime.Now.AddDays(-8), DueDate = DateTime.Now.AddDays(2), Status = "Due Soon" },
                new BorrowedBookItem { Title = "Brave New World", Author = "Aldous Huxley", BorrowDate = DateTime.Now.AddDays(-12), DueDate = DateTime.Now.AddDays(-1), Status = "Overdue" }
            };

            var returnedBooks = new List<ReturnedBookItem> {
                new ReturnedBookItem { Title = "The Alchemist", Author = "Paulo Coelho", BorrowDate = DateTime.Now.AddDays(-30), ReturnDate = DateTime.Now.AddDays(-5), Status = "On Time" },
                new ReturnedBookItem { Title = "The Hobbit", Author = "J.R.R. Tolkien", BorrowDate = DateTime.Now.AddDays(-25), ReturnDate = DateTime.Now.AddDays(-3), Status = "On Time" },
                new ReturnedBookItem { Title = "Dune", Author = "Frank Herbert", BorrowDate = DateTime.Now.AddDays(-20), ReturnDate = DateTime.Now.AddDays(-1), Status = "Returned Late" },
                new ReturnedBookItem { Title = "Foundation", Author = "Isaac Asimov", BorrowDate = DateTime.Now.AddDays(-40), ReturnDate = DateTime.Now.AddDays(-10), Status = "On Time" }
            };

            // Build transaction list
            var transactions = new List<TransactionItem>();

            // Add borrowed books (ReturnDate = null)
            foreach (var book in borrowedBooks)
            {
                string status = book.Status;
                transactions.Add(new TransactionItem {
                    Title = book.Title,
                    Author = book.Author,
                    BorrowDate = book.BorrowDate,
                    ReturnDate = null,
                    Status = status
                });
            }

            // Add returned books
            foreach (var book in returnedBooks)
            {
                transactions.Add(new TransactionItem {
                    Title = book.Title,
                    Author = book.Author,
                    BorrowDate = book.BorrowDate,
                    ReturnDate = book.ReturnDate,
                    Status = book.Status
                });
            }

            // Sort by BorrowDate descending (most recent first)
            transactions = transactions.OrderByDescending(t => t.BorrowDate).ToList();

            // Apply filters
            if (filter == "Borrowed")
            {
                transactions = transactions.Where(t => t.ReturnDate == null).ToList();
            }
            else if (filter == "Returned")
            {
                transactions = transactions.Where(t => t.ReturnDate != null).ToList();
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                transactions = transactions.Where(t => t.Title.ToLower().Contains(searchLower) ||
                                                     t.Author.ToLower().Contains(searchLower)).ToList();
            }

            var model = new UserTransactionHistoryViewModel {
                Transactions = transactions,
                Filter = filter,
                Search = search
            };

            return View(model);
        }

        // --- Books Management Merged from BooksController ---

        private static readonly Dictionary<string, List<CategoryBookItem>> BooksByCategory = new()
        {
            ["programming"] = new()
            {
                new CategoryBookItem { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", AvailableCopies = 5, CoverImageUrl = null },
                new CategoryBookItem { Id = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", AvailableCopies = 3, CoverImageUrl = null },
                new CategoryBookItem { Id = 3, Title = "Code Complete", Author = "Steve McConnell", AvailableCopies = 2, CoverImageUrl = null },
                new CategoryBookItem { Id = 4, Title = "JavaScript: The Good Parts", Author = "Douglas Crockford", AvailableCopies = 8, CoverImageUrl = null },
                new CategoryBookItem { Id = 5, Title = "Head First Design Patterns", Author = "Eric Freeman", AvailableCopies = 4, CoverImageUrl = null },
                new CategoryBookItem { Id = 6, Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", AvailableCopies = 0, CoverImageUrl = null },
            },
            ["database"] = new()
            {
                new CategoryBookItem { Id = 7, Title = "SQL in 10 Minutes", Author = "Ben Forta", AvailableCopies = 3, CoverImageUrl = null },
                new CategoryBookItem { Id = 8, Title = "Database System Concepts", Author = "Abraham Silberschatz", AvailableCopies = 2, CoverImageUrl = null },
                new CategoryBookItem { Id = 9, Title = "NoSQL Distilled", Author = "Pramod J. Sadalage", AvailableCopies = 1, CoverImageUrl = null },
            },
            ["networking"] = new()
            {
                new CategoryBookItem { Id = 10, Title = "Computer Networking", Author = "Andrew S. Tanenbaum", AvailableCopies = 4, CoverImageUrl = null },
                new CategoryBookItem { Id = 11, Title = "TCP/IP Illustrated", Author = "W. Richard Stevens", AvailableCopies = 2, CoverImageUrl = null },
            },
            ["web-development"] = new()
            {
                new CategoryBookItem { Id = 12, Title = "HTML & CSS: Design and Build Websites", Author = "Jon Duckett", AvailableCopies = 6, CoverImageUrl = null },
                new CategoryBookItem { Id = 13, Title = "React: Up and Running", Author = "Stoyan Stefanov", AvailableCopies = 0, CoverImageUrl = null },
                new CategoryBookItem { Id = 14, Title = "Vue.js 3 Cookbook", Author = "Daniel Khalil", AvailableCopies = 3, CoverImageUrl = null },
            },
            ["mobile-development"] = new()
            {
                new CategoryBookItem { Id = 15, Title = "Android Programming: The Big Nerd Ranch Guide", Author = "Bill Phillips", AvailableCopies = 2, CoverImageUrl = null },
                new CategoryBookItem { Id = 16, Title = "iOS Programming: The Big Nerd Ranch Guide", Author = "Christian Keur", AvailableCopies = 1, CoverImageUrl = null },
            },
            ["devops"] = new()
            {
                new CategoryBookItem { Id = 17, Title = "The Phoenix Project", Author = "Gene Kim", AvailableCopies = 4, CoverImageUrl = null },
                new CategoryBookItem { Id = 18, Title = "Site Reliability Engineering", Author = "Google SRE Team", AvailableCopies = 2, CoverImageUrl = null },
            },
            ["data-science"] = new()
            {
                new CategoryBookItem { Id = 19, Title = "Hands-On Machine Learning", Author = "Aurélien Géron", AvailableCopies = 3, CoverImageUrl = null },
                new CategoryBookItem { Id = 20, Title = "Data Science from Scratch", Author = "Joel Grus", AvailableCopies = 0, CoverImageUrl = null },
            },
            ["security"] = new()
            {
                new CategoryBookItem { Id = 21, Title = "The Web Application Hacker's Handbook", Author = "Dafydd Stuttard", AvailableCopies = 2, CoverImageUrl = null },
                new CategoryBookItem { Id = 22, Title = "Black Hat Python", Author = "Justin Seitz", AvailableCopies = 1, CoverImageUrl = null },
            },
            ["software-engineering"] = new()
            {
                new CategoryBookItem { Id = 23, Title = "Design Patterns", Author = "Erich Gamma et al.", AvailableCopies = 5, CoverImageUrl = null },
                new CategoryBookItem { Id = 24, Title = "Refactoring", Author = "Martin Fowler", AvailableCopies = 3, CoverImageUrl = null },
            }
        };

        private static readonly Dictionary<string, string> CategoryDisplayNames = new()
        {
            ["programming"] = "Programming",
            ["database"] = "Database",
            ["networking"] = "Networking",
            ["web-development"] = "Web Development",
            ["mobile-development"] = "Mobile Development",
            ["devops"] = "DevOps",
            ["data-science"] = "Data Science",
            ["security"] = "Security",
            ["software-engineering"] = "Software Engineering"
        };

        [HttpGet]
        public IActionResult Books(string? category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return RedirectToAction("Categories");
            }

            var categoryKey = category.ToLower();
            if (!BooksByCategory.ContainsKey(categoryKey))
            {
                return NotFound();
            }

            var books = BooksByCategory[categoryKey];
            var displayName = CategoryDisplayNames.GetValueOrDefault(categoryKey, category);

            var model = new UserCategoryBooksViewModel
            {
                CategoryName = displayName,
                Books = books
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Borrow(int bookId)
        {
            // Find book details
            string? bookTitle = null;
            string? author = null;
            foreach (var category in BooksByCategory.Values)
            {
                var book = category.FirstOrDefault(b => b.Id == bookId);
                if (book != null)
                {
                    bookTitle = book.Title;
                    author = book.Author;
                    break;
                }
            }

            if (bookTitle == null)
            {
                return NotFound();
            }

            var model = new BorrowBookViewModel
            {
                BookId = bookId,
                BookTitle = bookTitle,
                Author = author
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Borrow(BorrowBookViewModel model)
        {
            // Check if this is an AJAX request
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    return Json(new { success = false, errors });
                }
                return View(model);
            }

            // Find and decrement book availability
            foreach (var category in BooksByCategory.Values)
            {
                var book = category.FirstOrDefault(b => b.Id == model.BookId);
                if (book != null)
                {
                    if (book.AvailableCopies > 0)
                    {
                        book.AvailableCopies--;
                        if (isAjax)
                        {
                            return Json(new { success = true, message = $"Successfully borrowed '{book.Title}'!" });
                        }

                        TempData["SuccessMessage"] = $"Successfully borrowed '{book.Title}'!";
                        return RedirectToAction("BorrowedBooks");
                    }
                    else
                    {
                        if (isAjax)
                        {
                            return Json(new { success = false, message = "This book is currently out of stock." });
                        }
                        TempData["ErrorMessage"] = "This book is currently out of stock.";
                        return RedirectToAction("Books", new { category = GetCategoryByBookId(model.BookId) });
                    }
                }
            }

            TempData["ErrorMessage"] = "Book not found.";
            if (isAjax)
            {
                return Json(new { success = false, message = "Book not found." });
            }
            return RedirectToAction("Categories");
        }

        private string? GetCategoryByBookId(int bookId)
        {
            foreach (var kvp in BooksByCategory)
            {
                if (kvp.Value.Any(b => b.Id == bookId))
                {
                    return kvp.Key;
                }
            }
            return null;
        }
    }

    public class ProfileViewModel {
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
        public string Email { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string ExistingPictureUrl { get; set; }
    }

    public class UserInfoViewModel {
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
        public string Email { get; set; }
        public string MembershipTier { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsVerified { get; set; }
    }

    public class UserBorrowedBooksViewModel {
        public List<BorrowedBookItem> BorrowedBooks { get; set; }
    }

    public class BorrowedBookItem {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }

    public class UserReturnedBooksViewModel {
        public List<ReturnedBookItem> ReturnedBooks { get; set; }
    }

    public class ReturnedBookItem {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
    }

    public class UserTransactionHistoryViewModel {
        public List<TransactionItem> Transactions { get; set; }
        public string Filter { get; set; }
        public string Search { get; set; }
    }

    public class TransactionItem {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
    }
}
