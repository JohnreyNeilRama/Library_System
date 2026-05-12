/**
 * BorrowBookViewModel.cs
 * 
 * View model for the book borrowing process.
 * Contains book details and user information for the borrow form.
 * 
 * This is the updated version of the file.
 */

using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class BorrowBookViewModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }

        [Required(ErrorMessage = "Borrow date is required")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(14);

        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
