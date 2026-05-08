
namespace LibrarySystem.Models
{
    public class LoginViewModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
