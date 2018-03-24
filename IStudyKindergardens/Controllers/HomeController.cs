using IStudyKindergardens.HelpClasses;
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
    public class HomeController : Controller
    {
        private readonly ISiteUserManager _siteUserManager;
        private readonly IKindergardenManager _kindergardenManager;
        private readonly IRatingManager _ratingManager;
        private readonly IStatementManager _statementManager;

        public HomeController(IKindergardenManager kindergardenManager, IRatingManager ratingManager, ISiteUserManager siteUserManager, IStatementManager statementManager)
        {
            _siteUserManager = siteUserManager;
            _kindergardenManager = kindergardenManager;
            _ratingManager = ratingManager;
            _statementManager = statementManager;
        }

        [HttpGet]
        public ActionResult Index(string search)
        {
            ViewBag.Type = "MainSearch";
            return View(_kindergardenManager.GetFormatKindergardenListViewModel(false, User.Identity.GetUserId(), search, null, null, -1, -1));
        }

        [HttpGet]
        [Route("AdvancedSearch")]
        public ActionResult AdvancedSearch(string search, string searchBy, string sortBy)
        {
            ViewBag.Type = "AdvancedSearch";
            return View("Index", _kindergardenManager.GetFormatKindergardenListViewModel(false, User.Identity.GetUserId(), search, searchBy, sortBy, -1, -1));
        }

        [HttpPost]
        public JsonResult UploadPicture()
        {
            if (User.Identity.IsAuthenticated)
            {
                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    if (upload != null)
                    {
                        string expansion = upload.FileName.Split(new char[] { '.' }).Last();
                        string fileName = Guid.NewGuid().ToString() + '.' + expansion;
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
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult DeletePicture(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/Uploaded/Temp/") + id);
                }
                catch (Exception) { }
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult AddKindergardenForUser(string kindergardenId)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    return Json(_siteUserManager.SwitchAddRemoveKindergardenForSiteUser(kindergardenId, User.Identity.GetUserId()));
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        [HttpGet]
        [Route("MyKindergardens")]
        public ActionResult MyKindergardens()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index", _kindergardenManager.GetFormatKindergardenListViewModel(true, User.Identity.GetUserId(), null, null, null, -1, -1));
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Captcha()
        {
            string code = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();
            Session["code"] = code;
            CaptchaImage captcha = new CaptchaImage(code, 110, 50);

            Response.Clear();
            Response.ContentType = "image/jpeg";

            captcha.Image.Save(Response.OutputStream, ImageFormat.Jpeg);

            captcha.Dispose();
            return null;
        }

        [HttpGet]
        [Route("Apply/{id=}")]
        public ActionResult Apply(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Privilege> privileges = _statementManager.GetAllPrivileges();
                List<Group> groups = _statementManager.GetAllGroups();
                List<Kindergarden> kindergardens = _kindergardenManager.GetKindergardens().ToList();
                AddStatementViewModel model = new AddStatementViewModel { };
                model.Privileges = new List<PrivilegesInnerViewModel> { };
                model.Groups = new List<string> { };
                for (int i = 0; i < privileges.Count; i++)
                {
                    model.Privileges.Add(new PrivilegesInnerViewModel { Key = privileges[i].Value, Value = false });
                }
                for (int i = 0; i < groups.Count; i++)
                {
                    model.Groups.Add(groups[i].Value);
                }

                model.Kindergardens = new SelectList(kindergardens, "Id", "Name");

                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Apply/{id=}")]
        public ActionResult Apply(AddStatementViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (model.Captcha != Session["code"].ToString())
                {
                    ModelState.AddModelError("Captcha", "Неправильно введений код перевірки!");
                }
                if (model.Consent == false)
                {
                    ModelState.AddModelError("Consent", "Потрібне підтвердження!");
                }
                if (ModelState.IsValid)
                {
                    _statementManager.ApplyStatement(model, User.Identity.GetUserId());
                    return RedirectToAction("Index", "Home");
                }
                List<Kindergarden> kindergardens = _kindergardenManager.GetKindergardens().ToList();
                model.Kindergardens = new SelectList(kindergardens, "Id", "Name");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyStatements")]
        public ActionResult MyStatements()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")))
            {
                return View(_statementManager.GetFormatStatementListViewModel(User.Identity.GetUserId(), false, "none", true));
            }
            return RedirectToAction("Index", "Home");
        }

        private JsonResult SwitchIsRemoved(string statementId, bool isRemoved)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = _statementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergardenId)
            {
                try
                {
                    _statementManager.SwitchIsRemoved(intStatementId, isRemoved);
                    return Json(true);
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        private JsonResult SwitchStatus(string statementId, string status)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = _statementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergardenId)
            {
                try
                {
                    _statementManager.SwitchStatus(intStatementId, status);
                    return Json(true);
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        private JsonResult SwitchIsSelected(string statementId, bool isSelected)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = _statementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergardenId)
            {
                try
                {
                    _statementManager.SwitchIsSelected(intStatementId, isSelected);
                    return Json(true);
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        [HttpPost]
        public JsonResult FromRecycleBin(string statementId)
        {
            return SwitchIsRemoved(statementId, false);
        }

        [HttpPost]
        public JsonResult RejectStatement(string statementId)
        {
            return SwitchStatus(statementId, "Rejected");
        }

        [HttpPost]
        public JsonResult ApproveStatement(string statementId)
        {
            return SwitchStatus(statementId, "Approved");
        }

        [HttpPost]
        public JsonResult FromSelected(string statementId)
        {
            return SwitchIsSelected(statementId, false);
        }

        [HttpPost]
        public JsonResult ToSelected(string statementId)
        {
            return SwitchIsSelected(statementId, true);
        }

        [HttpPost]
        public JsonResult ToRecycleBin(string statementId)
        {
            return SwitchIsRemoved(statementId, true);
        }

        [HttpGet]
        [Route("RemoveStatement/{id}")]
        public ActionResult RemoveStatement(int id)
        {
            Statement statement = _statementManager.GetStatementById(id);
            if (User.Identity.IsAuthenticated &&
                ((User.IsInRole("User") && statement.SiteUserId == User.Identity.GetUserId())
                || (User.IsInRole("Admin") && statement.SiteUserId == User.Identity.GetUserId())))
            {
                Kindergarden kindergarden = _kindergardenManager.GetKindergardenById(statement.KindergardenId);
                SiteUser siteUser = _siteUserManager.GetSiteUserById(statement.SiteUserId);
                StatementListItemViewModel model = new StatementListItemViewModel
                {
                    Statement = statement,
                    UserPrivileges = _statementManager.GetUserPrivilegesByStatementId(id),
                    KindergardenName = kindergarden.Name,
                    UserName = String.Format("{0} {1} {2}", siteUser.Surname, siteUser.Name, siteUser.FathersName)
                };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("RemoveStatement")]
        public ActionResult RemoveStatement(StatementListItemViewModel model)
        {
            _statementManager.RemoveStatement(model.Statement.Id);
            return RedirectToAction("MyStatements", "Home");
        }

        [HttpPost]
        public JsonResult AddContactAjax(string addContactUserId)
        {
            _siteUserManager.AddContactUser(User.Identity.GetUserId(), addContactUserId);
            return Json(true);
        }

        [HttpPost]
        public JsonResult RemoveContactAjax(string removeContactUserId)
        {
            _siteUserManager.RemoveContactUser(User.Identity.GetUserId(), removeContactUserId);
            return Json(true);
        }

        [HttpPost]
        public JsonResult HideMessage(string messageIds)
        {
            try
            {
                if (messageIds.Length != 0)
                {
                    List<string> Ids = messageIds.Split(':').ToList();
                    if (Ids.Count != 0)
                    {
                        List<int> intIds = new List<int> { };
                        for (int i = 0; i < Ids.Count; i++)
                        {
                            intIds.Add(Convert.ToInt32(Ids[i]));
                        }
                        _siteUserManager.HideMessages(intIds, User.Identity.GetUserId());
                        return Json(true);
                    }
                }
            }
            catch { }
            return Json(false);
        }
    }
}