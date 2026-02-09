using Insure__Insurance_Company_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Insure__Insurance_Company_.Controllers
{
    public class AdminController : Controller
    {
        private readonly InsuredatabaseContext appDB;

        public AdminController(InsuredatabaseContext Db)
        {
            this.appDB = Db;
        }

        public IActionResult AdminDashboard()
        {
            ViewBag.IsAdmin = true;

            int totalUsers = appDB.Users.Count();

            int totalPolicies = appDB.Policies.Count();

            int totalUserPolicies = appDB.UserPolicies.Count();

            int totalNewsEmails = appDB.NewsletterSubscriptions.Count();


            decimal totalPayments = appDB.Payments.Sum(p => p.AmountPaid) ?? 0m;

            int pendingLoans = appDB.Loans.Count(l => l.Status == "Pending");


            ViewBag.TotalUsers = totalUsers;

            ViewBag.TotalPolicies = totalPolicies;

            ViewBag.TotalPayments = totalPayments;

            ViewBag.PendingLoans = pendingLoans;

            ViewBag.totalUserPolicies = totalUserPolicies;

            ViewBag.totalNewsEmails = totalNewsEmails;

            var fullName = HttpContext.Session.GetString("FullName");
            ViewBag.UserInitial = !string.IsNullOrEmpty(fullName) ? fullName.Substring(0, 1).ToUpper() : "U";


            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }

        public IActionResult AllUsers()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;
            var users = appDB.Users.ToList();
            return View(users);
        }

        public IActionResult EditUsers(int id)
        {

            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;


            var b = appDB.Users.FirstOrDefault(s => s.UserId == id);

            if (b == null)
            {
                return NotFound();
            }

            return View(b);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUsers(User s)
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var EmailExists = appDB.Users.Any(u => u.Email == s.Email && u.UserId != s.UserId);
            if (EmailExists)
            {
                ModelState.AddModelError("", "This email address is already registered. Please use a different one.");
            }

            var PhoneExists = appDB.Users.Any(u => u.Phone == s.Phone && u.UserId != s.UserId);
            if (PhoneExists)
            {
                ModelState.AddModelError("", "This phone number is already registered. Please use a different one.");
            }

            var NameExists = appDB.Users.Any(u => u.FullName == s.FullName && u.UserId != s.UserId);
            if (NameExists)
            {
                ModelState.AddModelError("", "This Full Name is already registered. Please use a different one.");
            }

            if (!ModelState.IsValid)
            {
                return View(s);
            }

            TempData["UpdateSuccess"] = "User updated successfully.";
            appDB.Users.Update(s);
            appDB.SaveChanges();
            return RedirectToAction("AllUsers");
        }


        public IActionResult DeleteUsers(int id)
        {
            ViewBag.IsNotDash = true;

            var a = HttpContext.Session.GetString("Email");
            if (a == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Account");

            var user = appDB.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound();

            if (user.Role == "Admin")
            {
                TempData["ErrorMessage"] = "Admins cannot be deleted.";
                return RedirectToAction("AllUsers");
            }

            var hasPolicies = appDB.UserPolicies.Any(up => up.UserId == id);
            if (hasPolicies)
            {
                TempData["deluserErrorMessage"] = "Cannot delete user because they have associated policies.";
                return RedirectToAction("AllUsers");
            }

            appDB.Users.Remove(user);
            appDB.SaveChanges();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("AllUsers");
        }



        public IActionResult Policies()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;

            var policies = appDB.Policies.Include(p => p.InsuranceType).ToList();

            return View(policies);
        }

        public IActionResult DelPolicy(int id)
        {
            ViewBag.IsNotDash = true;
            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var policy = appDB.Policies.FirstOrDefault(p => p.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }

            bool isPolicyInUse = appDB.UserPolicies.Any(up => up.PolicyId == id);
            if (isPolicyInUse)
            {
                TempData["PolicyErrorMessage"] = "Cannot delete this policy because it is assigned to one or more users.";
                return RedirectToAction("Policies");
            }

            TempData["DelPolMsg"] = "Policy deleted successfully.";
            appDB.Policies.Remove(policy);
            appDB.SaveChanges();

            return RedirectToAction("Policies");
        }



        public IActionResult AddPolicyWithInsurance()
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var vm = new PolicyWithInsuranceTypeViewModel
            {
                InsuranceTypesList = appDB.InsuranceTypes.Select(it => new SelectListItem
                {
                    Value = it.InsuranceTypeId.ToString(),
                    Text = it.InsuranceName
                }).ToList()
            };



            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPolicyWithInsurance(PolicyWithInsuranceTypeViewModel vm)
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var existingPolicy = appDB.Policies
                                    .FirstOrDefault(p => p.PolicyName == vm.PolicyName);
            if (existingPolicy != null)
            {
                ModelState.AddModelError("PolicyName", "This Policy Name already exists. Please choose a different name.");
            }

            if (!ModelState.IsValid)
            {
                vm.InsuranceTypesList = appDB.InsuranceTypes.Select(it => new SelectListItem
                {
                    Value = it.InsuranceTypeId.ToString(),
                    Text = it.InsuranceName
                }).ToList();

                return View(vm);
            }

            int insuranceTypeId;

            if (!string.IsNullOrEmpty(vm.NewInsuranceName))
            {
                var newInsuranceType = new InsuranceType
                {
                    InsuranceName = vm.NewInsuranceName,
                    Description = vm.NewDescription
                };
                appDB.InsuranceTypes.Add(newInsuranceType);
                appDB.SaveChanges();

                insuranceTypeId = newInsuranceType.InsuranceTypeId;
            }
            else
            {
                insuranceTypeId = vm.InsuranceTypeId ?? 0;

            }

            var policy = new Policy
            {
                InsuranceTypeId = insuranceTypeId,
                PolicyName = vm.PolicyName,
                TermYears = vm.TermYears,
                PremiumAmount = vm.PremiumAmount,
                CoverageAmount = vm.CoverageAmount,
                DurationInMonths = vm.DurationInMonth
            };

            TempData["AddPolMsg"] = "Policy added successfully.";
            appDB.Policies.Add(policy);
            appDB.SaveChanges();

            return RedirectToAction("Policies");
        }


        [HttpGet]
        public IActionResult AddInsType()
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInsType(InsuranceType insuranceType)
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var exists = appDB.InsuranceTypes
                               .Any(it => it.InsuranceName == insuranceType.InsuranceName);

            if (exists)
            {
                ModelState.AddModelError("InsuranceName", "This Insurance Name already exists. Please choose a different name.");
            }

            if (!ModelState.IsValid)
            {
                return View(insuranceType);
            }

            appDB.InsuranceTypes.Add(insuranceType);
            appDB.SaveChanges();

            TempData["AddInsTypeSuccess"] = "Insurance type added successfully.";

            return RedirectToAction("InsTypes");
        }


        public IActionResult InsTypes()
        {
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.IsNotDash = true;
            var insTypes = appDB.InsuranceTypes.ToList();
            return View(insTypes);
        }

        public IActionResult DelInsType(int id)
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");
            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var insuranceType = appDB.InsuranceTypes.FirstOrDefault(p => p.InsuranceTypeId == id);
            if (insuranceType == null)
            {
                return NotFound();
            }

            var isUsed = appDB.Policies.Any(p => p.InsuranceTypeId == id);
            if (isUsed)
            {
                TempData["InsTypeErrrorMsg"] = "Cannot delete this Insurance Type because it is associated with existing policies.";
                return RedirectToAction("InsTypes");
            }

            appDB.InsuranceTypes.Remove(insuranceType);
            appDB.SaveChanges();

            TempData["InsTypeSuccMsg"] = "Insurance Type deleted successfully.";
            return RedirectToAction("InsTypes");
        }

        public IActionResult EditInsType(int id)
        {

            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;


            var b = appDB.InsuranceTypes.FirstOrDefault(s => s.InsuranceTypeId == id);

            if (b == null)
            {
                return NotFound();
            }

            return View(b);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditInsType(InsuranceType s)
        {
            ViewBag.IsNotDash = true;
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var nameExists = appDB.InsuranceTypes
                .Any(i => i.InsuranceName == s.InsuranceName
                       && i.InsuranceTypeId != s.InsuranceTypeId);

            if (nameExists)
            {
                ModelState.AddModelError("InsuranceName",
                    "This Insurance Name already exists. Please use a different name.");
            }

            if (!ModelState.IsValid)
            {
                return View(s);
            }

            TempData["EditInsType"] = "Insurance Type updated successfully.";
            appDB.InsuranceTypes.Update(s);
            appDB.SaveChanges();

            return RedirectToAction("InsTypes");
        }


        public IActionResult Payments()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;

            var payments = appDB.Payments.Include(p => p.UserPolicy).ThenInclude(up => up.User).Include(p => p.UserPolicy).ThenInclude(up => up.Policy).ToList();


            return View(payments);
        }


        public IActionResult Loans()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.IsNotDash = true;

            var loans = appDB.Loans
                .Include(l => l.UserPolicy)
                    .ThenInclude(up => up.Policy) 
                .ToList();

            int pendingLoans = appDB.Loans.Count(l => l.Status == "Pending");
            int rejectedLoans = appDB.Loans.Count(l => l.Status == "Rejected");
            int approvedLoans = appDB.Loans.Count(l => l.Status == "Approved");
            int allLoans = appDB.Loans.Count();


            ViewBag.PenLoans = pendingLoans;
            ViewBag.RejLoans = rejectedLoans;
            ViewBag.AppLoans = approvedLoans;
            ViewBag.AllLoans = allLoans;

            return View(loans);

        }

        [HttpPost]
        public IActionResult ApproveLoan(int loanId)
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.IsNotDash = true;

            var loan = appDB.Loans.FirstOrDefault(l => l.LoanId == loanId);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = "Approved";
            appDB.SaveChanges();

            TempData["LoanApproved"] = "Loan approved successfully.";
            return RedirectToAction("Loans");
        }

        [HttpPost]
        public IActionResult RejectLoan(int loanId)
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.IsNotDash = true;


            var loan = appDB.Loans.FirstOrDefault(l => l.LoanId == loanId);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = "Rejected";
            appDB.SaveChanges();

            TempData["LoanRejected"] = "Loan rejected successfully.";
            return RedirectToAction("Loans");
        }

         public IActionResult UserPolicies()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;

            var userPolicies = appDB.UserPolicies
                .Include(up => up.User)
                .Include(up => up.Policy)
                .ThenInclude(p => p.InsuranceType)
                .ToList();


            return View(userPolicies);
        }

        public IActionResult NewsLetterEmails()
        {
            var a = HttpContext.Session.GetString("Email");

            if (a == null)
            {

                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsNotDash = true;
            var newsEmails = appDB.NewsletterSubscriptions.ToList();
            return View(newsEmails);
        }

        
    }
}
