using Insure__Insurance_Company_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Insure__Insurance_Company_.Controllers
{
    public class UserController : Controller
    {
        private readonly InsuredatabaseContext appDB;

        public UserController(InsuredatabaseContext Db)
        {
            this.appDB = Db;
        }

        public IActionResult UserDashboard()
        {
            ViewBag.IsAdmin = true;
            var fullName = HttpContext.Session.GetString("FullName");
            ViewBag.UserInitial = !string.IsNullOrEmpty(fullName) ? fullName.Substring(0, 1).ToUpper() : "U";

            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "User")
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            ViewBag.IsUser = true;

            var email = HttpContext.Session.GetString("Email");

            if (email == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Account");

            if (user.DateOfBirth.HasValue)
            {
                var dob = user.DateOfBirth.Value;
                var today = DateTime.Today;
                var age = today.Year - dob.Year;

                if (dob.Date > today.AddYears(-age))
                {
                    age--;
                }

                ViewBag.UserAge = age;
            }
            else
            {
                ViewBag.UserAge = null;
            }


            return View(user);
        }


        public IActionResult EditProfile(int id)
        {

            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("Role") != "User") return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();


            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User s)
        {

            var email = HttpContext.Session.GetString("Email");
            if (email == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("Role") != "User") return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;



            if (string.IsNullOrWhiteSpace(s.FullName))
                ModelState.AddModelError("FullName", "Full Name is required.");

            if (string.IsNullOrWhiteSpace(s.Email))
                ModelState.AddModelError("Email", "Email is required.");

            if (string.IsNullOrWhiteSpace(s.Phone))
                ModelState.AddModelError("Phone", "Phone number is required.");

            if (s.DateOfBirth == default)
                ModelState.AddModelError("DateOfBirth", "Date of Birth is required.");

            if (s.DateOfBirth > DateTime.Today)
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");

            if (appDB.Users.Any(u => u.Email == s.Email && u.UserId != s.UserId))
                ModelState.AddModelError("Email", "This email is already registered.");

            if (appDB.Users.Any(u => u.Phone == s.Phone && u.UserId != s.UserId))
                ModelState.AddModelError("Phone", "This phone number is already registered.");

            if (!ModelState.IsValid)
                return View(s);

            var userInDb = appDB.Users.FirstOrDefault(u => u.UserId == s.UserId);
            if (userInDb == null) return NotFound();

            userInDb.FullName = s.FullName;
            userInDb.Email = s.Email;
            userInDb.Phone = s.Phone;
            userInDb.Address = s.Address;
            userInDb.DateOfBirth = s.DateOfBirth;


            appDB.SaveChanges();

            TempData["ProfileUpdateSuccess"] = "Profile updated successfully.";
            return RedirectToAction("UserProfile");
        }

        [HttpGet]
        public IActionResult AllPolicies()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsUser = true;

            var policies = appDB.Policies.Include(p => p.InsuranceType).ToList();

            if (policies == null)
            {
                return NotFound();

            }
            return View(policies);
        }

        [HttpGet]
        public IActionResult BuyPolicy(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);
            var policy = appDB.Policies
                .Include(p => p.InsuranceType)
                .FirstOrDefault(p => p.PolicyId == id);

            if (user == null || policy == null)
                return NotFound();

            ViewBag.User = user;
            ViewBag.Policy = policy;

            return View();
        }



        [HttpPost]
        public IActionResult CalculatePremium(int policyId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Login", "Account");

            if (user.DateOfBirth == null)
            {
                TempData["DOBError"] = "Date of birth is required to calculate premium.";
                return RedirectToAction("EditProfile");
            }

            var policy = appDB.Policies
                .Include(p => p.InsuranceType)
                .FirstOrDefault(p => p.PolicyId == policyId);

            if (policy == null)
                return NotFound();

            if (policy.PremiumAmount == null)
            {
                TempData["PremAmoError"] = "Premium amount is not set for this policy.";
                return RedirectToAction("BuyPolicy");
            }

            if (user.DateOfBirth.HasValue)
            {
                var dob = user.DateOfBirth.Value;
                var today = DateTime.Today;
                int age = today.Year - dob.Year;

                if (dob.Date > today.AddYears(-age))
                {
                    age--;
                }

                ViewBag.UserAge = age;

                decimal basePremium = policy.PremiumAmount ?? 0m;
                decimal premium;

                if (age < 25)
                    premium = basePremium * 0.8m; //20% discount
                else if (age <= 40)
                    premium = basePremium;
                else if (age <= 60)
                    premium = basePremium * 1.4m; //40% increase
                else
                    premium = basePremium * 2m; //100% increase

                ViewBag.User = user;
                ViewBag.Policy = policy;
                ViewBag.Age = age;
                ViewBag.FinalPremium = premium;
            }
            else
            {
                ViewBag.UserAge = null;
                ViewBag.User = user;
                ViewBag.Policy = policy;
                ViewBag.Age = null;
                ViewBag.FinalPremium = policy.PremiumAmount ?? 0m;
            }

            return View("BuyPolicy");

        }

        [HttpPost]
        public IActionResult ConfirmPolicy(int policyId, decimal finalPremium, int age)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);
            var policy = appDB.Policies.FirstOrDefault(p => p.PolicyId == policyId);

            if (user == null || policy == null)
                return NotFound();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var expiredPolicies = appDB.UserPolicies
                .Where(up => up.UserId == user.UserId && up.Status == "Active" && up.EndDate < today)
                .ToList();

            foreach (var expPolicy in expiredPolicies)
            {
                expPolicy.Status = "Expired";
            }
            appDB.SaveChanges();

            bool alreadyExists = appDB.UserPolicies.Any(up =>
                up.UserId == user.UserId &&
                up.PolicyId == policyId &&
                up.Status == "Active");

            if (alreadyExists)
            {
                TempData["ActivePolErr"] = "You already have an active policy of this type.";
                return RedirectToAction("MyPolicies");
            }

            var userPolicy = new UserPolicy
            {
                UserId = user.UserId,
                PolicyId = policyId,
                StartDate = today,
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddYears(policy.TermYears ?? 1)),
                Status = "Active",
                FinalPremium = finalPremium,
                AgeAtPurchase = age
            };

            appDB.UserPolicies.Add(userPolicy);
            appDB.SaveChanges();

            TempData["PolicySuccess"] = "Policy purchased successfully!";
            return RedirectToAction("MyPolicies");
        }



        public IActionResult MyPolicies()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);

            var policies = appDB.UserPolicies
                .Include(up => up.Policy)
                .ThenInclude(p => p.InsuranceType)
                .Where(up => up.UserId == user.UserId)
                .ToList();

            // Get payments count per policy
            var paymentsCount = appDB.Payments
                .Where(p => p.UserPolicy.UserId == user.UserId)
                .GroupBy(p => p.UserPolicyId)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.PaymentsCount = paymentsCount;

            return View(policies);
        }




        public IActionResult PayPremium(int userPolicyId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var policy = appDB.UserPolicies
                .Include(up => up.Policy)
                .FirstOrDefault(up => up.UserPolicyId == userPolicyId);

            if (policy == null)
                return NotFound();

            ViewBag.PaymentSuccess = TempData["PaymentSuccess"];
            ViewBag.PaymentError = TempData["PaymentError"];

            return View(policy);
        }


        [HttpPost]
        public IActionResult PayPremium(int userPolicyId, string paymentMode)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var policy = appDB.UserPolicies
                .Include(up => up.Policy)
                .FirstOrDefault(up => up.UserPolicyId == userPolicyId);

            if (policy == null)
                return NotFound();

            var now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;



            var payment = new Payment
            {
                UserPolicyId = userPolicyId,
                AmountPaid = policy.FinalPremium,
                PaymentDate = DateOnly.FromDateTime(now),
                PaymentMonth = currentMonth,
                PaymentYear = currentYear,
                PaymentMode = paymentMode
            };

            appDB.Payments.Add(payment);
            appDB.SaveChanges();

            TempData["PaymentSuccess"] = "Premium paid successfully ✅";
            return RedirectToAction("MyPolicies", "User");
        }


        public IActionResult PaymentHistory()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var policies = appDB.UserPolicies
                .Include(up => up.Policy)
                .Where(up => up.UserId == user.UserId)
                .ToList();

            var paymentsByPolicy = appDB.Payments
                .Where(p => p.UserPolicy.UserId == user.UserId)
                .GroupBy(p => p.UserPolicyId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new PaymentViewModel
                    {
                        PaymentDate = p.PaymentDate ?? default(DateOnly),
                        AmountPaid = p.AmountPaid ?? 0m,
                        PaymentMode = p.PaymentMode
                    }).OrderByDescending(p => p.PaymentDate).ToList()
                );

            var model = policies.Select(p =>
            {
                paymentsByPolicy.TryGetValue(p.UserPolicyId, out var paymentsList);
                int paidCount = paymentsList?.Count ?? 0;
                int total = p.Policy.DurationInMonths;
                int leftCount = total - paidCount;
                if (leftCount < 0) leftCount = 0;

                return new UserPolicyPaymentHistoryViewModel
                {
                    UserPolicyId = p.UserPolicyId,
                    PolicyName = p.Policy.PolicyName,
                    StartDate = p.StartDate ?? DateOnly.MinValue,
                    EndDate = p.EndDate ?? DateOnly.MinValue,
                    Status = p.Status,
                    DurationInMonths = total,
                    PremiumsPaidCount = paidCount,
                    PremiumsLeftCount = leftCount,
                    Payments = paymentsList ?? new List<PaymentViewModel>()
                };
            }).ToList();

            return View(model);
        }


      
           public IActionResult ApplyLoan()
        {
            var email = HttpContext.Session.GetString("Email");

            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;

            var user = appDB.Users.First(u => u.Email == email);

            ViewBag.UserPolicies = appDB.UserPolicies
                .Include(up => up.Policy)
                .Where(up => up.UserId == user.UserId && up.Status == "Active")
                .Select(up => new SelectListItem
                {
                    Value = up.UserPolicyId.ToString(),
                    Text = up.Policy.PolicyName
                })
                .ToList();

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyLoan(Loan loan)
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            var user = appDB.Users.First(u => u.Email == email);

            bool ownsPolicy = appDB.UserPolicies
                .Any(up => up.UserPolicyId == loan.UserPolicyId && up.UserId == user.UserId);

            if (!ownsPolicy)
                return Unauthorized();


            loan.Status = "Pending";
            loan.LoanDate = DateOnly.FromDateTime(DateTime.Now);
            loan.InterestRate ??= 10;

            appDB.Loans.Add(loan);
            appDB.SaveChanges();

            TempData["Success"] = "Loan application submitted successfully!";
            return RedirectToAction("MyLoans");
        }


 



        public IActionResult MyLoans()
        {
            var email = HttpContext.Session.GetString("Email");
            if (email == null || HttpContext.Session.GetString("Role") != "User")
                return RedirectToAction("Login", "Account");

            ViewBag.IsUser = true;


            var user = appDB.Users.FirstOrDefault(u => u.Email == email);

            var loans = appDB.Loans
                .Include(l => l.UserPolicy)
                .ThenInclude(up => up.Policy)
                .Where(l => l.UserPolicy.UserId == user.UserId)
                .OrderByDescending(l => l.LoanDate)
                .ToList();

            return View(loans);
        }



    }


}
