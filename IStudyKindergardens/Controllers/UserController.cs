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
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
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
                    if (siteUser == null)
                    {
                        throw new Exception();
                    }
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
                return RedirectToAction("UserProfile", "User", new { id });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Contacts")]
        public ActionResult Contacts()
        {
            return View(_siteUserManager.GetContactUsers(User.Identity.GetUserId()));
        }

        [HttpGet]
        [Route("AddContact")]
        public ActionResult AddContact()
        {
            return View(_siteUserManager.GetAddContactListViewModel(User.Identity.GetUserId()));
        }

        [HttpGet]
        [Route("WriteMessage")]
        public ActionResult WriteMessage()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                List<SiteUser> siteUsers = _siteUserManager.GetContactUsers(User.Identity.GetUserId()).ToList();
                siteUsers.Insert(0, new SiteUser { Id = "-1", Surname = "Виберіть користувача..." });
                SendMessageViewModel model = new SendMessageViewModel
                {
                    UserContacts = new SelectList(siteUsers, "Id", "FullName")
                };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("WriteMessage")]
        public ActionResult WriteMessage(SendMessageViewModel model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                _siteUserManager.WriteMessage(User.Identity.GetUserId(), model);
                return RedirectToAction("SentMessages", "User");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyMessages")]
        public ActionResult MyMessages()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                List<MessageUserListItemModel> model = _siteUserManager.GetAllMessages(User.Identity.GetUserId());
                ViewBag.IsSent = false;
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("SentMessages")]
        public ActionResult SentMessages()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                List<MessageUserListItemModel> model = _siteUserManager.GetAllSentMessages(User.Identity.GetUserId());
                ViewBag.IsSent = true;
                return View("MyMessages", model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyMessages/{id}")]
        public ActionResult MyMessages(int id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                int messageId = -1;
                try
                {
                    messageId = Convert.ToInt32(id);
                }
                catch
                {
                    return RedirectToAction("MyMessages", "User");
                }
                _siteUserManager.ReadMessage(messageId, User.Identity.GetUserId());
                ReMessageList model = _siteUserManager.GetReMessageList(User.Identity.GetUserId(), messageId);
                if (model != null)
                {
                    return View("ReWriteMessage", model);
                }
                return RedirectToAction("MyMessages", "User");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("MyMessages")]
        public ActionResult MyMessages(ReMessageList model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                SendMessageViewModel modelToWrite = new SendMessageViewModel { ToUserId = model.ReceiverId, Text = model.NewText, Theme = model.Theme };
                _siteUserManager.WriteMessage(User.Identity.GetUserId(), modelToWrite, model.ReMessageId);
                return RedirectToAction("MyMessages", "User");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}