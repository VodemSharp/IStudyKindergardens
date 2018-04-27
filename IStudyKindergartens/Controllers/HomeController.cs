using IStudyKindergartens.HelpClasses;
using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Kindergartens;
using IStudyKindergartens.Models.Statements;
using IStudyKindergartens.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISiteUserManager _siteUserManager;
        private readonly IKindergartenManager _KindergartenManager;
        private readonly IRatingManager _ratingManager;
        private readonly IStatementManager _statementManager;
        private readonly IMessageManager _messageManager;
        private readonly IApplicationManager _applicationManager;

        public HomeController(IKindergartenManager KindergartenManager, IRatingManager ratingManager, ISiteUserManager siteUserManager, IStatementManager statementManager, IMessageManager messageManager, IApplicationManager applicationManager)
        {
            _siteUserManager = siteUserManager;
            _KindergartenManager = KindergartenManager;
            _ratingManager = ratingManager;
            _statementManager = statementManager;
            _messageManager = messageManager;
            _applicationManager = applicationManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Link = "/";
            return View();
        }

        [HttpGet]
        [Route("Search")]
        public ActionResult Search(string search, int currentPage = 1)
        {
            ViewBag.Type = "MainSearch";
            List<KindergartenListItemViewModel> kindergartenListItemViewModels = KindergartenManager.GetFormatKindergartenListViewModel(false, User.Identity.GetUserId(), search, null, null, (currentPage - 1) * 5, 5);
            ViewBag.CountOfPages = Convert.ToInt32(kindergartenListItemViewModels.Count / 5) + 1;
            ViewBag.CurrentPage = currentPage;
            return View(kindergartenListItemViewModels);
        }

        [HttpGet]
        [Route("AdvancedSearch")]
        public ActionResult AdvancedSearch(string search, string searchBy, string sortBy)
        {
            ViewBag.Type = "AdvancedSearch";
            return View("Search", KindergartenManager.GetFormatKindergartenListViewModel(false, User.Identity.GetUserId(), search, searchBy, sortBy, -1, -1));
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
        public JsonResult AddKindergartenForUser(string KindergartenId)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")))
            {
                try
                {
                    return Json(SiteUserManager.SwitchAddRemoveKindergartenForSiteUser(KindergartenId, User.Identity.GetUserId()));
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        [HttpGet]
        [Route("MyKindergartens")]
        public ActionResult MyKindergartens()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Link = "/MyKindergartens";
                return View("Index", KindergartenManager.GetFormatKindergartenListViewModel(true, User.Identity.GetUserId(), null, null, null, -1, -1));
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
                List<Privilege> privileges = StatementManager.GetAllPrivileges();
                List<Group> groups = StatementManager.GetAllGroups();
                List<Kindergarten> Kindergartens = KindergartenManager.GetKindergartens().ToList();
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

                model.Kindergartens = new SelectList(Kindergartens, "Id", "Name");

                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Apply/{id=}")]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(AddStatementViewModel model)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")))
            {
                if (ModelState.IsValid)
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
                        StatementManager.ApplyStatement(model, User.Identity.GetUserId());
                        return RedirectToAction("Index", "Home");
                    }
                    List<Kindergarten> Kindergartens = KindergartenManager.GetKindergartens().ToList();
                    model.Kindergartens = new SelectList(Kindergartens, "Id", "Name");
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("MyStatements")]
        public ActionResult MyStatements()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")))
            {
                return View(StatementManager.GetFormatStatementListViewModel(User.Identity.GetUserId(), false, "none", true));
            }
            return RedirectToAction("Index", "Home");
        }

        private JsonResult SwitchIsRemoved(string statementId, bool isRemoved)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = StatementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergartenId || User.IsInRole("Admin"))
            {
                try
                {
                    StatementManager.SwitchIsRemoved(intStatementId, isRemoved);
                    return Json(true);
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        private JsonResult SwitchStatus(string statementId, string status)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = StatementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergartenId || User.IsInRole("Admin"))
            {
                try
                {
                    StatementManager.SwitchStatus(intStatementId, status);
                    return Json(true);
                }
                catch (Exception) { }
            }
            return Json("Error");
        }

        private JsonResult SwitchIsSelected(string statementId, bool isSelected)
        {
            int intStatementId = Convert.ToInt32(statementId);
            Statement statement = StatementManager.GetStatementById(intStatementId);
            if (User.Identity.IsAuthenticated && User.Identity.GetUserId() == statement.KindergartenId || User.IsInRole("Admin"))
            {
                try
                {
                    StatementManager.SwitchIsSelected(intStatementId, isSelected);
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
            try
            {
                Statement statement = StatementManager.GetStatementById(id);
                if (User.Identity.IsAuthenticated &&
                    ((User.IsInRole("User") && statement.SiteUserId == User.Identity.GetUserId())
                    || (User.IsInRole("Admin") && statement.SiteUserId == User.Identity.GetUserId())))
                {
                    Kindergarten Kindergarten = KindergartenManager.GetKindergartenById(statement.KindergartenId);
                    SiteUser siteUser = SiteUserManager.GetSiteUserById(statement.SiteUserId);
                    StatementListItemViewModel model = new StatementListItemViewModel
                    {
                        Statement = statement,
                        UserPrivileges = StatementManager.GetUserPrivilegesByStatementId(id),
                        KindergartenName = Kindergarten.Name,
                        UserName = String.Format("{0} {1} {2}", siteUser.Surname, siteUser.Name, siteUser.FathersName)
                    };
                    return View(model);
                }
            }
            catch { }
            return RedirectToAction("MyStatements", "Home");
        }

        [HttpPost]
        [Route("RemoveStatement")]
        public ActionResult RemoveStatement(StatementListItemViewModel model)
        {
            try
            {
                Statement statement = StatementManager.GetStatementById(model.Statement.Id);
                if (User.Identity.IsAuthenticated &&
                    ((User.IsInRole("User") && statement.SiteUserId == User.Identity.GetUserId())
                    || (User.IsInRole("Admin") && statement.SiteUserId == User.Identity.GetUserId())))
                {
                    StatementManager.RemoveStatement(model.Statement.Id);
                    return RedirectToAction("MyStatements", "Home");
                }
            }
            catch { }
            return RedirectToAction("MyStatements", "Home");
        }

        [HttpPost]
        public JsonResult AddContactAjax(string addContactUserId)
        {
            SiteUserManager.AddContactUser(User.Identity.GetUserId(), addContactUserId);
            return Json(true);
        }

        [HttpPost]
        public JsonResult RemoveContactAjax(string removeContactUserId)
        {
            SiteUserManager.RemoveContactUser(User.Identity.GetUserId(), removeContactUserId);
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
                        MessageManager.HideMessages(intIds, User.Identity.GetUserId());
                        return Json(true);
                    }
                }
            }
            catch { }
            return Json(false);
        }

        #region RemoteMethods

        public JsonResult IsEmailExist(string Email)
        {
            if (ApplicationManager.IsEmailExist(Email))
            {
                return Json("Користувач з таким email вже існує!",
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckDate(string DateOfBirth)
        {
            if (!DateTime.TryParse(DateOfBirth, out DateTime parsedDate))
            {
                return Json("Не коректний формат дати народження!",
                    JsonRequestBehavior.AllowGet);
            }
            else if (DateTime.Now < parsedDate)
            {
                return Json("Це дата народження майбутнього!",
                    JsonRequestBehavior.AllowGet);
            }
            else if (new DateTime(1900, 1, 1) > parsedDate)
            {
                return Json("Ви не можете бути настільки старі!",
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Properties

        public IKindergartenManager KindergartenManager
        {
            get
            {
                return _KindergartenManager;
            }
        }

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

        public IStatementManager StatementManager
        {
            get
            {
                return _statementManager;
            }
        }

        public IRatingManager RatingManager
        {
            get
            {
                return _ratingManager;
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