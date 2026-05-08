/**
 * RegisterViewModel.cs
 * 
 * View model for the user registration page.
 * 
 * This is the updated version of the file.
 */

namespace LibrarySystem.Models
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
    }
}
