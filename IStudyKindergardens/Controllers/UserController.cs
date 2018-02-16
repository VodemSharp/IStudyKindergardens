using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class UserController : Controller
    {
        private ISiteUserManager _siteUserManager;

        public UserController(ISiteUserManager siteUserManager)
        {
            _siteUserManager = siteUserManager;
        }

        public ISiteUserManager SiteUserManager
        {
            get
            {
                return _siteUserManager;
            }
        }

        [HttpGet]
        [Route("User/{id}")]
        public ActionResult UserProfile(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin")))
            {
                try
                {
                    SiteUser siteUser = SiteUserManager.GetSiteUserById(id);
                    ViewBag.PhoneNumber = siteUser.ApplicationUser.PhoneNumber.Substring(4);
                    try
                    {
                        string PictureUID = SiteUserManager.GetPictureUIDById(id);
                        if (PictureUID == null)
                        {
                            throw new Exception();
                        }
                        ViewBag.Picture = "/Images/Uploaded/Source/" + PictureUID;
                    }
                    catch (Exception)
                    {
                        ViewBag.Picture = "/Images/Default/anonym.png";
                    }
                    return View(siteUser);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("User") && id == User.Identity.GetUserId()) || User.IsInRole("Admin")))
            {
                try
                {
                    SiteUser siteUser = SiteUserManager.GetSiteUserById(id);
                    string picture;
                    string phoneNumber = siteUser.ApplicationUser.PhoneNumber.Substring(4);
                    try
                    {
                        string PictureUID = SiteUserManager.GetPictureUIDById(id);
                        if (PictureUID == null)
                        {
                            throw new Exception();
                        }
                        picture = "/Images/Uploaded/Source/" + PictureUID;
                    }
                    catch (Exception)
                    {
                        picture = null;
                    }

                    return View(new EditUserViewModel { PictureName = picture, Surname = siteUser.Surname, Name = siteUser.Name, FathersName = siteUser.FathersName, Email = siteUser.ApplicationUser.Email, PhoneNumber = phoneNumber, DateOfBirth = siteUser.DateOfBirth });
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model, string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("User") && id == User.Identity.GetUserId()) || User.IsInRole("Admin")))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                try
                {
                    SiteUserManager.EditSiteUser(model, id, Server);
                }
                catch (Exception) { }
                return RedirectToAction("UserProfile", "User", new { id = id });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}