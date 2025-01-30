using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TM.BL.Models;
using WebApp.ViewModels;
using WebApp.Security;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    
    public class UserController : Controller
    {
        private readonly TaskMgmtContext _context;

        public UserController(TaskMgmtContext context)
        {
            _context = context;
        }
        // GET: UserController
        [Authorize(Roles = "Menager")]
        public ActionResult Index()
        {
            try
            {
                var UserVms = _context.Users.Select(x => new UserVM
                {
                    Id = x.Id,
                    Username = x.Username,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Role = x.Role,
                    Email = x.Email,
                    Phone = x.Phone,
                }).ToList();

                return View(UserVms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Login(string returnUrl)
        {
            var loginVm = new LoginVM
            {
                ReturnUrl = returnUrl
            };

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVm)
        {
            // Try to get a user from database
            var existingUser =
                _context
                    .Users
                    .FirstOrDefault(x => x.Username == loginVm.Username);

            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Check is password hash matches
            var b64hash = Security.PasswordHashProvider.GetHash(loginVm.Password, existingUser.PwdSalt);
            if (b64hash != existingUser.PwdHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, loginVm.Username),
                new Claim(ClaimTypes.Role, existingUser.Role ?? "Emploeyee")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            // We need to wrap async code here into synchronous since we don't use async methods
            System.Threading.Tasks.Task.Run(async () =>
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties)
            ).GetAwaiter().GetResult();

            if (loginVm.ReturnUrl != null)
                return LocalRedirect(loginVm.ReturnUrl);
            else if (existingUser.Role == "Admin")
                return RedirectToAction("Index", "AdminHome");
            else if (existingUser.Role == "Manager")
                return RedirectToAction("Index", "AdminHome");
            else if (existingUser.Role == "Employee")
                return RedirectToAction("Index", "Home");
            else
                return View();
        }

        public IActionResult Logout()
        {
            System.Threading.Tasks.Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserVM userVm)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userVm.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userVm.Password, b64salt);

                //Congradulation you are a manager (if you have no friends)
                if(_context.Users.Count()  <= 0)
                {
                    userVm.Role = "Menager";
                }

                // Create user from DTO and hashed password
                var user = new User
                {
                    Id = userVm.Id,
                    Username = userVm.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userVm.FirstName,
                    LastName = userVm.LastName,
                    Email = userVm.Email,
                    Phone = userVm.Phone,
                    Role = userVm.Role
                };

                // Add user and save changes to database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
                userVm.Id = user.Id;

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        public IActionResult ProfileDetails()
        {
            var username = HttpContext.User.Identity.Name;

            var userDb = _context.Users.First(x => x.Username == username);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone,
            };

            return View(userVm);
        }

        [Authorize]
        public IActionResult ProfileEdit()
        {
            var username = HttpContext.User.Identity.Name;
            var userDb = _context.Users.First(x => x.Username == username);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone,
            };

            return View(userVm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProfileEdit(int id, UserVM userVm)
        {
            var userDb = _context.Users.First(x => x.Username == userVm.Username);
            userDb.FirstName = userVm.FirstName;
            userDb.LastName = userVm.LastName;
            userDb.Email = userVm.Email;
            userDb.Phone = userVm.Phone;

            _context.SaveChanges();

            return RedirectToAction("ProfileDetails");
        }
        public JsonResult GetProfileData(int id)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            return Json(new
            {
                userDb.FirstName,
                userDb.LastName,
                userDb.Email,
                userDb.Phone,
            });
        }



        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
