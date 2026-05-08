using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using LibrarySystem.Services;
using LibrarySystem.Models;

namespace LibrarySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILibraryService _libraryService;

        public AccountController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            // Redirect already-authenticated users to their dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Dashboard", "Admin");
                return RedirectToAction("Dashboard", "User");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                if (_libraryService.ValidateUser(model.UserNameOrEmail, model.Password))
                {
                    var user = _libraryService.GetUserByUserName(model.UserNameOrEmail) ??
                               _libraryService.GetUserByEmail(model.UserNameOrEmail);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user!.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.GivenName, user.FullName),
                        new Claim("IsAdmin", user.IsAdmin.ToString())
                    };

                    if (user.IsAdmin)
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return LocalRedirect(returnUrl);

                    if (user.IsAdmin)
                        return RedirectToAction("Dashboard", "Admin");

                    return RedirectToAction("Dashboard", "User");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            // Redirect already-authenticated users to their dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Dashboard", "Admin");
                return RedirectToAction("Dashboard", "User");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new InMemoryUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FullName = model.FullName,
                    IdNumber = model.IdNumber,
                    Course = model.Course,
                    YearLevel = model.YearLevel,
                    MembershipTier = "Standard",
                    ProfilePictureUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(model.FullName)}&background=0d6efd&color=fff&size=80",
                    IsVerified = true,
                    IsAdmin = false
                };

                if (_libraryService.CreateUser(user))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.GivenName, user.FullName),
                        new Claim("IsAdmin", "False")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Dashboard", "User");
                }

                ModelState.AddModelError(string.Empty, "Username or email already exists.");
            }
            return View(model);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}
