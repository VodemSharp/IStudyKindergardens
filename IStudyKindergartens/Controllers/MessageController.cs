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
    public class MessageController : Controller
    {
        private readonly ISiteUserManager _siteUserManager;
        private readonly IMessageManager _messageManager;

        public MessageController(ISiteUserManager siteUserManager,IMessageManager messageManager)
        {
            _siteUserManager = siteUserManager;
            _messageManager = messageManager;
        }

        [HttpGet]
        [Route("WriteMessage")]
        public ActionResult WriteMessage()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
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
        public ActionResult WriteMessage(SendMessageViewModel model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin") || User.IsInRole("Administator")))
            {
                MessageManager.WriteMessage(User.Identity.GetUserId(), model);
                return RedirectToAction("SentMessages", "User");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyMessages")]
        public ActionResult MyMessages()
        {
            if (User.Identity.IsAuthenticated)
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
            if (User.Identity.IsAuthenticated)
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
            if (User.Identity.IsAuthenticated)
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
                MessageManager.ReadMessage(messageId, User.Identity.GetUserId());
                ReMessageList model = MessageManager.GetReMessageList(User.Identity.GetUserId(), messageId);
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
            if (User.Identity.IsAuthenticated)
            {
                SendMessageViewModel modelToWrite = new SendMessageViewModel { ToUserId = model.ReceiverId, Text = model.NewText, Theme = model.Theme };
                MessageManager.WriteMessage(User.Identity.GetUserId(), modelToWrite, model.ReMessageId);
                return RedirectToAction("MyMessages", "User");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/WriteTo")]
        public ActionResult WriteTo(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                SendMessageToViewModel model = new SendMessageToViewModel
                {
                    ToUserId = id,
                    ToUser = SiteUserManager.GetSiteUserById(id).FullName
                };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("/WriteTo")]
        public ActionResult WriteTo(SendMessageToViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                MessageManager.WriteToMessage(User.Identity.GetUserId(), model);
                return RedirectToAction("SentMessages", "User");
            }
            return RedirectToAction("Index", "Home");
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

        #endregion
    }
}