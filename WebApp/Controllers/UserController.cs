using ClassLibrary.DataContext;
using ClassLibrary.Models;
using ClassLibrary.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly MedicalDbContext _context;
        public UserController(MedicalDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string url)
        {
            ViewData["HideNavbar"] = true;
            var userLoginVM = new UserLoginVM 
            {
                ReturnUrl = url
            };
            return View(userLoginVM);
        }

        [HttpPost]
        public IActionResult Login(UserLoginVM userLoginVM)
        {
            var existingUser = _context.Users.Include(x => x.Role)
                               .FirstOrDefault(x => x.UserName == userLoginVM.UserName);
            if (existingUser == null)
            { 
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var b64Hash = PasswordHashProvider.GetHash(userLoginVM.Password, existingUser.PasswordSalt);
            if (b64Hash != existingUser.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.UserName),
                new Claim(ClaimTypes.Role, existingUser.Role.RoleName)
            };
            var claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();
            
            Task.Run(async() => await HttpContext.SignInAsync(
                               CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                                             authProperties)).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(userLoginVM.ReturnUrl))
            {
                return LocalRedirect(userLoginVM.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            } 
        }

        public IActionResult Logout()
        {
            Task.Run(async () => await HttpContext.SignOutAsync(
                               CookieAuthenticationDefaults.AuthenticationScheme)).GetAwaiter().GetResult();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            ViewData["HideNavbar"] = true;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Register(UserVM userVM)
        {
            var trimmedUsername = userVM.UserName.Trim();
            if (_context.Users.Any(x => x.UserName == trimmedUsername))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            return RedirectToAction("ConfirmRegistration", userVM);
        }

        public ActionResult ConfirmRegistration(UserVM userVM)
        {
            return View(userVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CompletedRegistration(UserVM userVM)
        { 
            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash(userVM.Password, b64salt);

            var user = new User
            {
                UserName = userVM.UserName,
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                Email = userVM.Email,
                PhoneNumber = userVM.PhoneNumber,
                PasswordHash = b64hash,
                PasswordSalt = b64salt,
                RoleId = 2 // Default role for new users
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            TempData["Success"] = "Registration completed successfully. You can now log in.";
            return RedirectToAction("Login");
        }
    }
}
