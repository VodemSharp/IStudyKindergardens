using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Kindergartens;
using IStudyKindergartens.Models.Ratings;
using IStudyKindergartens.Models.Statements;
using IStudyKindergartens.Models.Users;
using IStudyKindergartens.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ISiteUserManager _siteUserManager;
        private readonly IKindergartenManager _KindergartenManager;
        private readonly IRatingManager _ratingManager;
        private readonly IStatementManager _statementManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ISiteUserManager siteUserManager, IKindergartenManager KindergartenManager, RoleManager<IdentityRole> roleManager, IRatingManager ratingManager, IStatementManager statementManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _siteUserManager = siteUserManager;
            _KindergartenManager = KindergartenManager;
            _roleManager = roleManager;
            _ratingManager = ratingManager;
            _statementManager = statementManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Users()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(SiteUserManager.GetSiteUsers());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(AddUserViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = "+38 " + model.PhoneNumber };
                    var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        SiteUserManager.AddSiteUser(model, user.Id, Server);
                        return RedirectToAction("Users", "Admin");
                    }

                    AddErrors(result);
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DeleteUser(string id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                SiteUser siteUser = SiteUserManager.GetSiteUserById(id);
                if (siteUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.SNF = String.Format("{0} {1} {2}", siteUser.Surname, siteUser.Name, siteUser.FathersName);
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(DeleteUserViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                SiteUserManager.DeleteSiteUser(model.Id, Server);
                return RedirectToAction("Users", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Kindergartens()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(KindergartenManager.GetKindergartens());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DeleteKindergarten(string id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Kindergarten Kindergarten = KindergartenManager.GetKindergartenById(id);
                if (Kindergarten == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(new DeleteKindergartenViewModel { Id = Kindergarten.Id, Name = Kindergarten.Name });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteKindergarten(DeleteKindergartenViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                KindergartenManager.DeleteKindergarten(model.Id, Server);
                return RedirectToAction("Kindergartens", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddKindergarten()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKindergarten(AddKindergartenViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    string password = Guid.NewGuid().ToString("N");
                    var result = UserManager.Create(user, password);
                    //
                    try
                    {
                        MailMessage msg = new MailMessage("istudy.network@gmail.com", model.Email, "IStudy password", GetAnswer(model.Email, password))
                        {
                            IsBodyHtml = true
                        };
                        SmtpClient sc = new SmtpClient("smtp.gmail.com", 587)
                        {
                            UseDefaultCredentials = false
                        };
                        NetworkCredential cre = new NetworkCredential("istudy.network@gmail.com", "istudyrepublika");
                        sc.Credentials = cre;
                        sc.EnableSsl = true;
                        sc.Send(msg);
                    }
                    catch (Exception)
                    {

                    }
                    //

                    if (result.Succeeded)
                    {
                        KindergartenManager.AddKindergarten(model, user.Id, Server);
                        UserManager.AddToRole(user.Id, "Administrator");
                        return RedirectToAction("Kindergartens", "Admin");
                    }

                    AddErrors(result);
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        private string GetAnswer(string email, string password)
        {
            return "<span style='font-size: 16px; font-weight: bold;'>Your email:</span><br><span style='font-size: 16px;'>" + email + "</span><br><span style='font-size: 16px; font-weight: bold;'>Your password:</span><br><span style='font-size: 16px;'>" + password + "</span><br>";
        }

        [HttpGet]
        public ActionResult Statements()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(StatementManager.GetFormatStatementListViewModel());
            }
            return RedirectToAction("Index", "Home");
        }

        #region Questions

        [HttpGet]
        public ActionResult Questions()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                List<Question> questions = RatingManager.GetAllQuestions();
                return View(questions);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddQuestion()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddQuestion(Question question)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                RatingManager.AddQuestion(question);
                return RedirectToAction("Questions", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult EditQuestion(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Question question = RatingManager.GetQuestionById(id);
                return View(question);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditQuestion(Question question)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                RatingManager.EditQuestion(question);
                return RedirectToAction("Questions", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult RemoveQuestion(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(new RemoveQuestionViewModel { Id = id, Value = RatingManager.GetQuestionById(id).Value });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult RemoveQuestion(RemoveQuestionViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (RatingManager.GetQuestionById(model.Id) != null)
                {
                    RatingManager.RemoveQuestion(model.Id);
                    //Видалення всіх відповідей
                }
                return RedirectToAction("Questions", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Privileges

        [HttpGet]
        public ActionResult Privileges()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                List<Privilege> privileges = StatementManager.GetAllPrivileges();
                return View(privileges);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddPrivilege()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddPrivilege(Privilege privilege)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                StatementManager.AddPrivilege(privilege);
                return RedirectToAction("Privileges", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult EditPrivilege(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Privilege privilege = StatementManager.GetPrivilegeById(id);
                return View(privilege);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditPrivilege(Privilege privilege)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                StatementManager.EditPrivilege(privilege);
                return RedirectToAction("Privileges", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult RemovePrivilege(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(new RemovePrivilegeViewModel { Id = id, Value = StatementManager.GetPrivilegeById(id).Value });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult RemovePrivilege(RemovePrivilegeViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (StatementManager.GetPrivilegeById(model.Id) != null)
                {
                    StatementManager.RemovePrivilege(model.Id);
                }
                return RedirectToAction("Privileges", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Groups

        [HttpGet]
        public ActionResult Groups()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                List<Group> groups = StatementManager.GetAllGroups();
                return View(groups);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddGroup()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AddGroup(Group group)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                StatementManager.AddGroup(group);
                return RedirectToAction("Groups", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult EditGroup(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Group group = StatementManager.GetGroupById(id);
                return View(group);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditGroup(Group group)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                StatementManager.EditGroup(group);
                return RedirectToAction("Groups", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult RemoveGroup(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(new RemoveGroupViewModel { Id = id, Value = StatementManager.GetGroupById(id).Value });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult RemoveGroup(RemoveGroupViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (StatementManager.GetGroupById(model.Id) != null)
                {
                    StatementManager.RemoveGroup(model.Id);
                }
                return RedirectToAction("Groups", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Properties
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
        }

        public ISiteUserManager SiteUserManager
        {
            get
            {
                return _siteUserManager;
            }
        }

        public IKindergartenManager KindergartenManager
        {
            get
            {
                return _KindergartenManager;
            }
        }

        public IRatingManager RatingManager
        {
            get
            {
                return _ratingManager;
            }
        }

        public IStatementManager StatementManager
        {
            get
            {
                return _statementManager;
            }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                return _roleManager;
            }
        }

        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}