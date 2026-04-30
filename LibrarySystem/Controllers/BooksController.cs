using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        private static readonly Dictionary<string, List<CategoryBookItem>> BooksByCategory = new()
        {
            ["programming"] = new()
            {
                new CategoryBookItem { Id = 1, Title = "C# 10.0 in a Nutshell", Author = "Joseph Albahari", AvailableCopies = 5, CoverImageUrl = null },
                new CategoryBookItem { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", AvailableCopies = 0, CoverImageUrl = null },
                new CategoryBookItem { Id = 3, Title = "Head First Java", Author = "Kathy Sierra", AvailableCopies = 3, CoverImageUrl = null },
                new CategoryBookItem { Id = 4, Title = "Python Crash Course", Author = "Eric Matthes", AvailableCopies = 2, CoverImageUrl = null },
                new CategoryBookItem { Id = 5, Title = "JavaScript: The Good Parts", Author = "Douglas Crockford", AvailableCopies = 4, CoverImageUrl = null },
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
                new CategoryBookItem { Id = 21, Title = "The Web Application Hacker''s Handbook", Author = "Dafydd Stuttard", AvailableCopies = 2, CoverImageUrl = null },
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
        public IActionResult Index(string? category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return RedirectToAction("Categories", "User");
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
                return View(model); // Fallback for non-AJAX (should not happen in modal)
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
                        var categoryKey = GetCategoryByBookId(model.BookId);

                        if (isAjax)
                        {
                            return Json(new { success = true, message = $"Successfully borrowed ''{book.Title}''!" });
                        }

                        TempData["SuccessMessage"] = $"Successfully borrowed ''{book.Title}''!";
                        return RedirectToAction("BorrowedBooks", "User");
                    }
                    else
                    {
                        if (isAjax)
                        {
                            return Json(new { success = false, message = "This book is currently out of stock." });
                        }
                        TempData["ErrorMessage"] = "This book is currently out of stock.";
                        return RedirectToAction("Index", new { category = GetCategoryByBookId(model.BookId) });
                    }
                }
            }

            TempData["ErrorMessage"] = "Book not found.";
            if (isAjax)
            {
                return Json(new { success = false, message = "Book not found." });
            }
            return RedirectToAction("Categories", "User");
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
}

