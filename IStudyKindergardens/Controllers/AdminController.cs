using IStudyKindergardens.Models;
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
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private IDataRepository dataRepository;

        public AdminController(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Users
        public ActionResult Users()
        {
            return View(dataRepository.GetSiteUsers());
        }

        // GET: Admin/AddUser
        public ActionResult AddUser()
        {
            return View();
        }

        // POST: Admin/AddUser
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = "+38 " + model.PhoneNumber };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    dataRepository.AddSiteUser(model, user.Id, Server);
                    return RedirectToAction("Users", "Admin");
                }

                AddErrors(result);
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult UploadPicture()
        {
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    string expansion = upload.FileName.Split(new char[] { '.' }).Last();
                    string fileName = Guid.NewGuid().ToString() + '.' + expansion;
                    dataRepository.AddTempPicture(fileName);
                    string path = Server.MapPath("~/Images/Uploaded/Temp/" + fileName);
                    upload.SaveAs(path);
                    try
                    {
                        using (MemoryStream memory = new MemoryStream())
                        {
                            using (Image img = Image.FromFile(path))
                            {
                                PropertyItem item = img.GetPropertyItem(274);
                                if (item.Value[0] == 3)
                                {
                                    img.RotateFlip(RotateFlipType.Rotate180FlipXY);
                                }
                                else if (item.Value[0] == 6)
                                {
                                    img.RotateFlip(RotateFlipType.Rotate270FlipXY);
                                }
                                else if (item.Value[0] == 8)
                                {
                                    img.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                }
                                img.Save(memory, ImageFormat.Jpeg);
                            }
                            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                            {
                                byte[] bytes = memory.ToArray();
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                    catch (Exception) { }
                    return Json(fileName);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult DeletePicture(string id)
        {
            try
            {
                System.IO.File.Delete(Server.MapPath("~/Images/Uploaded/Temp/") + id);
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        // GET: Admin/Kindergardens
        public ActionResult Kindergardens()
        {
            return View();
        }

        // GET: Admin/Comments
        public ActionResult Comments()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

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