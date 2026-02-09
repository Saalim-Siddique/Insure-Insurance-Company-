using Insure__Insurance_Company_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insure__Insurance_Company_.Controllers
{
    public class AccountController : Controller
    {
        private readonly InsuredatabaseContext appDB;

        public AccountController(InsuredatabaseContext Db)
        {
            this.appDB = Db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string loginValue, string password)
        {
            var user = appDB.Users.FirstOrDefault(u =>
                (u.Email == loginValue || u.FullName == loginValue)
                && u.Password == password
            );

            if (user != null)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("UserDashboard", "User");
                }
            }

            ViewBag.ErrorMessage = "Invalid email/full name or password";
            return View();
        }



        public IActionResult Sign_Up()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sign_Up(User user)
        {
            var EmailExists = appDB.Users.Any(u => u.Email == user.Email);
            if (EmailExists)
            {
                ModelState.AddModelError("", "This email address is already registered. Please use a different one.");
            }

            var PhoneExists = appDB.Users.Any(u => u.Phone == user.Phone);
            if (PhoneExists)
            {
                ModelState.AddModelError("", "This phone number is already registered. Please use a different one.");
            }

            var NameExists = appDB.Users.Any(u => u.FullName == user.FullName);
            if (NameExists)
            {
                ModelState.AddModelError("", "This Full Name is already registered. Please use a different one.");
            }

            if (user.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Role = "User";
            user.CreatedAt = DateTime.Now;

            appDB.Users.Add(user);
            appDB.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";

            return RedirectToAction("Login");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index" , "Home");
        }
    }
}
