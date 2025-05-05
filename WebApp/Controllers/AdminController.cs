using ClassLibrary.DataContext;
using ClassLibrary.Interfaces;
using ClassLibrary.Models;
using ClassLibrary.Services;
using ClassLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly MedicalDbContext _context;
        private readonly IUserService _userService;

        public AdminController(ILogger<AdminController> logger, MedicalDbContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService; 
        }

        //List users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users.OrderBy(u => u.UserId).ToList());
        }

        public IActionResult Create()
        {
            ViewData["HideNavbar"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserVM userVm)
        { 
            var trimmedUsername = userVm.UserName.Trim();
            if (_context.Users.Any(x => x.UserName.Equals(trimmedUsername)))
            {
                ModelState.AddModelError("", "This username already exists");
                return View();
            }

            return RedirectToAction("ConfirmedCreation", userVm);
        }

        public ActionResult ConfirmedCreation(UserVM userVm)
        {
            return View(userVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletedCreation(UserVM userVM)
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

            _context.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("Index");
        }

        //Edit user
        public async Task<IActionResult> Edit(int id)
        { 
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.UserName = form["UserName"];
            existingUser.Email = form["Email"];
            existingUser.RoleId = int.Parse(form["RoleId"]);

            if (string.IsNullOrWhiteSpace(existingUser.UserName) || string.IsNullOrWhiteSpace(existingUser.Email))
            {
                ModelState.AddModelError("", "Username and Email are required.");
                return View(existingUser);
            }

            await _userService.UpdateUserAsync(existingUser);
            return RedirectToAction(nameof(Index));

        }

        //Delete user
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        { 
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        //Show user details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //Password change
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var changepwVm = new ChangePasswordVM
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
            };

            return View(changepwVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(changePasswordVM.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(changePasswordVM.NewPassword, b64salt);
                user.PasswordHash = b64hash;
                user.PasswordSalt = b64salt;

                await _userService.UpdateUserAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(changePasswordVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 1);
            if (role == null)
            {
                return NotFound();
            }

            user.RoleId = role.RoleId;
            user.Role = role;
            await _userService.UpdateUserAsync(user);
            TempData["SuccessMessage"] = "User promoted to admin successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
