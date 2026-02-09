using Insure__Insurance_Company_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Insure__Insurance_Company_.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly InsuredatabaseContext appDB;

        public HomeController(InsuredatabaseContext Db)
        {
            this.appDB = Db;
        }

        public IActionResult Index()
        {
            var instypes = appDB.InsuranceTypes.ToList();

            ViewBag.InsuranceTypes = instypes;

            return View(instypes);
        }

        public IActionResult InsuranceDetails(int id)
        {
            var insurance = appDB.InsuranceTypes
                .FirstOrDefault(i => i.InsuranceTypeId == id);

            if (insurance == null)
            {
                return NotFound();
            }

            var policies = appDB.Policies
                .Where(p => p.InsuranceTypeId == id)
                .ToList();

            ViewBag.Insurance = insurance;
            ViewBag.Policies = policies;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Service()
        {
            var instypes = appDB.InsuranceTypes.ToList();

            ViewBag.InsuranceTypes = instypes;

            return View(instypes);
        }

        public IActionResult Features()
        {
            return View();
        }

        public IActionResult Appointment()
        {
            return View();
        }

        public IActionResult TeamMembers()
        {
            return View();
        }

        public IActionResult Testimonial()
        {
            return View();
        }

        public IActionResult Page_404()
        {
            return View();
        }

        public IActionResult Contact_Us()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SubscribeNewsletter(string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                TempData["NewsletterError"] = "Please enter a valid email address.";
                return RedirectToAction("Index");
            }

            bool exists = appDB.NewsletterSubscriptions.Any(n => n.Email == email);
            if (exists)
            {
                TempData["NewsletterError"] = "This email is already subscribed.";
                return RedirectToAction("Index");
            }

            var subscription = new NewsletterSubscription
            {
                Email = email
            };

            appDB.NewsletterSubscriptions.Add(subscription);
            appDB.SaveChanges();

            TempData["NewsletterSuccess"] = "Thank you for subscribing!";

            return RedirectToAction("Index");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
