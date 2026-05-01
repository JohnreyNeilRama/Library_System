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

        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
