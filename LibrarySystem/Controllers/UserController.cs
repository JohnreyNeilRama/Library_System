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
