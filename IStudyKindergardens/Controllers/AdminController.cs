﻿using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ISiteUserManager _siteUserManager;
        private readonly IKindergardenManager _kindergardenManager;

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ISiteUserManager siteUserManager, IKindergardenManager kindergardenManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _siteUserManager = siteUserManager;
            _kindergardenManager = kindergardenManager;
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
        public ActionResult Kindergardens()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(KindergardenManager.GetKindergardens());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddKindergarden()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKindergarden(AddKindergardenViewModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                return RedirectToAction("Kindergardens", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

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

        public IKindergardenManager KindergardenManager
        {
            get
            {
                return _kindergardenManager;
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