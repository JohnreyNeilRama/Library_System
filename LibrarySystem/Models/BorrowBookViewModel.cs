using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class BorrowBookViewModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID number is required")]
        public string IdNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course is required")]
        public string Course { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year level is required")]
        public string YearLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
    }
}
