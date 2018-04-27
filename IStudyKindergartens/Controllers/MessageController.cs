using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Messages;
using IStudyKindergartens.Models.Users;
using IStudyKindergartens.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Controllers
{
    //Перевірка на те, чи повідомлення належить юзеру в репозиторіях
    public class MessageController : Controller
    {
        private readonly ISiteUserManager _siteUserManager;
        private readonly IMessageManager _messageManager;
        private readonly IApplicationManager _applicationManager;

        public MessageController(ISiteUserManager siteUserManager, IMessageManager messageManager, IApplicationManager applicationManager)
        {
            _siteUserManager = siteUserManager;
            _messageManager = messageManager;
            _applicationManager = applicationManager;
        }

        [HttpGet]
        [Route("WriteMessage")]
        public ActionResult WriteMessage()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin")))
            {
                List<SiteUser> siteUsers = SiteUserManager.GetContactUsers(User.Identity.GetUserId()).ToList();
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
        [ValidateAntiForgeryToken]
        public ActionResult WriteMessage(SendMessageViewModel model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin")))
            {
                if (ModelState.IsValid)
                {
                    if (model.ToUserId != "-1")
                    {
                        if (model.Theme == null)
                        {
                            model.Theme = "<Без теми>";
                        }
                        MessageManager.WriteMessage(User.Identity.GetUserId(), model);
                        return RedirectToAction("SentMessages", "Message");
                    }
                    else
                    {
                        ModelState.AddModelError("ToUserId", "Виберіть користувача!");
                        List<SiteUser> siteUsers = SiteUserManager.GetContactUsers(User.Identity.GetUserId()).ToList();
                        siteUsers.Insert(0, new SiteUser { Id = "-1", Surname = "Виберіть користувача..." });
                        model.UserContacts = new SelectList(siteUsers, "Id", "FullName");
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyMessages")]
        public ActionResult MyMessages()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                List<MessageUserListItemModel> model = MessageManager.GetAllMessages(User.Identity.GetUserId());
                ViewBag.IsSent = false;
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("SentMessages")]
        public ActionResult SentMessages()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                List<MessageUserListItemModel> model = MessageManager.GetAllSentMessages(User.Identity.GetUserId());
                ViewBag.IsSent = true;
                return View("MyMessages", model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyMessages/{id}")]
        public ActionResult MyMessages(int id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                int messageId = -1;
                try
                {
                    messageId = Convert.ToInt32(id);
                }
                catch
                {
                    return RedirectToAction("MyMessages", "Message");
                }
                MessageManager.ReadMessage(messageId, User.Identity.GetUserId());
                ReMessageList model = MessageManager.GetReMessageList(User.Identity.GetUserId(), messageId);
                if (model != null)
                {
                    return View("ReWriteMessage", model);
                }
                return RedirectToAction("MyMessages", "Message");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("MyMessages")]
        [ValidateAntiForgeryToken]
        public ActionResult MyMessages(ReMessageList model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                if (ModelState.IsValid)
                {
                    SendMessageViewModel modelToWrite = new SendMessageViewModel { ToUserId = model.ReceiverId, Text = model.NewText, Theme = model.Theme };
                    MessageManager.WriteMessage(User.Identity.GetUserId(), modelToWrite, model.ReMessageId);
                    ViewBag.IsSent = true;
                    return RedirectToAction("MyMessages", "Message");
                }
                else
                {
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("WriteTo/{id}")]
        public ActionResult WriteTo(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                SendMessageToViewModel model = new SendMessageToViewModel
                {
                    ToUserId = id,
                    ToUser = ApplicationManager.GetApplicationUserNameById(id)
                };
                return View(model);
            }
            ViewBag.IsSent = true;
            return RedirectToAction("MyMessages", "Message");
        }

        [HttpPost]
        [Route("WriteTo/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult WriteTo(SendMessageToViewModel model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administrator")))
            {
                if (ModelState.IsValid)
                {
                    MessageManager.WriteToMessage(User.Identity.GetUserId(), model);
                    return RedirectToAction("SentMessages", "Message");
                }
                else
                {
                    return View(model);
                }
            }
            ViewBag.IsSent = true;
            return RedirectToAction("MyMessages", "Message");
        }

        #region Properties

        public IMessageManager MessageManager
        {
            get
            {
                return _messageManager;
            }
        }

        public ISiteUserManager SiteUserManager
        {
            get
            {
                return _siteUserManager;
            }
        }

        public IApplicationManager ApplicationManager
        {
            get
            {
                return _applicationManager;
            }
        }

        #endregion
    }
}