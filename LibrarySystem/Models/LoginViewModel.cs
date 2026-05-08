/**
 * LoginViewModel.cs
 * 
 * View model for the login page.
 * 
 * This is the updated version of the file.
 */

namespace LibrarySystem.Models
{
    public class LoginViewModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
