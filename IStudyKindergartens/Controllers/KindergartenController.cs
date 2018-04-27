using IStudyKindergartens.HelpClasses;
using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Kindergartens;
using IStudyKindergartens.Models.Ratings;
using IStudyKindergartens.Models.Statements;
using IStudyKindergartens.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Controllers
{
    public class KindergartenController : Controller
    {
        private readonly ISiteUserManager _siteUserManager;
        private readonly IKindergartenManager _KindergartenManager;
        private readonly IRatingManager _ratingManager;
        private readonly IStatementManager _statementManager;

        public KindergartenController(IKindergartenManager KindergartenManager, IRatingManager ratingManager, IStatementManager statementManager, ISiteUserManager siteUserManager)
        {
            _KindergartenManager = KindergartenManager;
            _ratingManager = ratingManager;
            _statementManager = statementManager;
            _siteUserManager = siteUserManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Kindergarten/{id}")]
        public ActionResult KindergartenProfile(string id)
        {
            try
            {
                Kindergarten Kindergarten = KindergartenManager.GetKindergartenById(id);
                if (Kindergarten == null)
                {
                    throw new Exception();
                }
                try
                {
                    string PictureUID = KindergartenManager.GetPictureUIDById(id);
                    if (PictureUID == null)
                    {
                        throw new Exception();
                    }
                    ViewBag.Picture = "/Images/Uploaded/Source/" + PictureUID;
                }
                catch (Exception)
                {
                    ViewBag.Picture = "/Images/Default/anonymKindergarten.jpg";
                }
                ViewBag.Blocks = KindergartenManager.GetDescriptionBlocksById(id);
                return View(Kindergarten);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                try
                {
                    Kindergarten Kindergarten = KindergartenManager.GetKindergartenById(id);
                    if (Kindergarten == null)
                    {
                        throw new Exception();
                    }
                    try
                    {
                        string PictureUID = KindergartenManager.GetPictureUIDById(id);
                        if (PictureUID == null)
                        {
                            throw new Exception();
                        }
                        ViewBag.Picture = "/Images/Uploaded/Source/" + PictureUID;
                    }
                    catch (Exception)
                    {
                        ViewBag.Picture = "/Images/Default/anonymKindergarten.jpg";
                    }
                    List<DescriptionBlock> descriptionBlocks = KindergartenManager.GetDescriptionBlocksById(id);
                    EditKindergartenViewModel model = new EditKindergartenViewModel { Id = id, Name = Kindergarten.Name, DescriptionBlocks = descriptionBlocks };
                    return View(model);
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
        public ActionResult Edit(EditKindergartenViewModel model, string content)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                if (ModelState.IsValid)
                {
                    content = content.Replace("$ENTER$", "\r\n");
                    List<DescriptionBlock> descriptionBlocks = new List<DescriptionBlock> { };
                    List<string> blocks = content.Split(new string[] { "$EDGE$" }, StringSplitOptions.None).ToList();
                    List<string> temp;
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        temp = blocks[i].Split(new string[] { "$SLASH$" }, StringSplitOptions.None).ToList();
                        switch (temp[0])
                        {
                            case "text":
                                descriptionBlocks.Add(new DescriptionBlockText { KindergartenId = model.Id, Header = temp[1], Body = temp[2] });
                                break;
                            case "text-image":
                                descriptionBlocks.Add(new DescriptionBlockTextImage { KindergartenId = model.Id, Image = temp[1], Header = temp[2], Body = temp[3] });
                                break;
                        }
                    }
                    KindergartenManager.EditKindergarten(descriptionBlocks, model.Id, Server, model);
                    return RedirectToAction("KindergartenProfile", "Kindergarten", new { id = model.Id });
                }
                else
                {
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && id == User.Identity.GetUserId()) || User.IsInRole("Admin")))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangeAvatar(string id, string left, string top, string right, string bottom, string src)
        {
            try
            {
                if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
                {
                    int left_int = Convert.ToInt32(left.Split(new char[] { '.' })[0]);
                    int top_int = Convert.ToInt32(top.Split(new char[] { '.' })[0]);
                    int right_int = Convert.ToInt32(right.Split(new char[] { '.' })[0]);
                    int bottom_int = Convert.ToInt32(bottom.Split(new char[] { '.' })[0]);
                    string expansion = src.Split(new char[] { '.' }).Last();
                    string fileName = Guid.NewGuid().ToString() + '.' + expansion;
                    if (right_int > left_int && bottom_int > top_int)
                    {
                        if ((right_int - left_int) > (bottom_int - top_int))
                        {
                            right_int--;
                        }
                        if ((right_int - left_int) < (bottom_int - top_int))
                        {
                            bottom_int--;
                        }
                        if ((right_int - left_int) != (bottom_int - top_int))
                        {
                            throw new Exception();
                        }
                        CustomImage.Crop(Image.FromFile(Server.MapPath("~" + src)), new Rectangle(left_int, top_int, right_int - left_int, bottom_int - top_int), fileName, Server);
                        KindergartenManager.AddPreviewPicture(id, fileName, Server);
                    }
                    else
                    {
                        throw new Exception();
                    }
                    return RedirectToAction("KindergartenProfile", "Kindergarten", id);
                }
            }
            catch (Exception) { }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeAddress(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                ChangeAddressViewModel model = new ChangeAddressViewModel { Address = KindergartenManager.GetKindergartenById(id).Address, AltAddress = KindergartenManager.GetKindergartenClaimValue(id, "AltAddress") };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAddress(ChangeAddressViewModel model)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                if (ModelState.IsValid)
                {
                    KindergartenManager.AddKindergartenClaimWithDel(model.Id, "AltAddress", model.AltAddress);
                    KindergartenManager.EditKindergartenAddress(model.Id, model.Address);
                    return RedirectToAction("KindergartenProfile", "Kindergarten", new { id = model.Id });
                }
                else
                {
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult EditShortInfo(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                EditShortInfoViewModel model = new EditShortInfoViewModel { Id = id, ShortInfo = KindergartenManager.GetKindergartenClaimValue(id, "ShortInfo") };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShortInfo(EditShortInfoViewModel model)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                if (ModelState.IsValid)
                {
                    KindergartenManager.AddKindergartenClaimWithDel(model.Id, "ShortInfo", model.ShortInfo);
                    return RedirectToAction("KindergartenProfile", "Kindergarten", new { id = model.Id });
                }
                else
                {
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Rate(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")))
            {
                List<QuestionRating> questionRatings = RatingManager.GetListOfQuestionRatingById(id, User.Identity.GetUserId());
                List<int> questionRatingValues = new List<int> { };
                for (int i = 0; i < questionRatings.Count; i++)
                {
                    questionRatingValues.Add(questionRatings[i].Rating);
                }
                QuestionRatingViewModel model = new QuestionRatingViewModel
                {
                    Id = id,
                    Questions = RatingManager.GetAllQuestions(),
                    Ratings = questionRatingValues,
                    Comment = RatingManager.GetCommentById(id, User.Identity.GetUserId())
                };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Rate(QuestionRatingViewModel model, string jsRatings, string jsQuestions)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("User")) && jsRatings != "" && jsQuestions != "")
            {
                List<string> rating = jsRatings.Split(new char[] { ':' }).ToList();
                List<string> questionIds = jsQuestions.Split(new char[] { ':' }).ToList();
                List<int> intRating = new List<int> { };
                List<int> intQuestionIds = new List<int> { };
                for (int i = 0; i < rating.Count; i++)
                {
                    intRating.Add(Convert.ToInt32(rating[i]));
                    intQuestionIds.Add(Convert.ToInt32(questionIds[i]));
                }
                model.Comment = model.Comment ?? "";
                RatingManager.Rate(model.Id, intQuestionIds, intRating, model.Comment, User.Identity.GetUserId());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Statements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "All";
                return View(StatementManager.GetFormatStatementListViewModel(id, true));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SelectedStatements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "Selected";
                return View("~/Views/Kindergarten/Statements.cshtml", StatementManager.GetFormatStatementListViewModel(id, true, "Selected"));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult RemovedStatements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "Removed";
                return View("~/Views/Kindergarten/Statements.cshtml", StatementManager.GetFormatStatementListViewModel(id, true, "Removed"));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ApprovedStatements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "Approved";
                return View("~/Views/Kindergarten/Statements.cshtml", StatementManager.GetFormatStatementListViewModel(id, true, "Approved"));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult RejectedStatements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "Rejected";
                return View("~/Views/Kindergarten/Statements.cshtml", StatementManager.GetFormatStatementListViewModel(id, true, "Rejected"));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult NotConsideredStatements(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator") && User.Identity.GetUserId() == id)))
            {
                ViewBag.Id = id;
                ViewBag.Type = "NotConsidered";
                return View("~/Views/Kindergarten/Statements.cshtml", StatementManager.GetFormatStatementListViewModel(id, true, "NotConsidered"));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult SendToEmail(int id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || (User.IsInRole("Administrator"))))
            {
                try
                {
                    Statement statement = StatementManager.GetStatementById(id);
                    if (User.Identity.IsAuthenticated &&
                        ((User.IsInRole("Administrator") && statement.KindergartenId == User.Identity.GetUserId())
                        || (User.IsInRole("Admin"))))
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
                    };
                }
                catch (Exception) { }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SendToEmail(string kindergartenId, int id, string email)
        {
            Statement statement = StatementManager.GetStatementById(id);
            if (User.Identity.IsAuthenticated &&
                ((User.IsInRole("Administrator") && statement.KindergartenId == User.Identity.GetUserId())
                || (User.IsInRole("Admin"))))
            {
                try
                {
                    Kindergarten Kindergarten = KindergartenManager.GetKindergartenById(statement.KindergartenId);
                    SiteUser siteUser = SiteUserManager.GetSiteUserById(statement.SiteUserId);
                    StatementListItemViewModel model = new StatementListItemViewModel
                    {
                        Statement = statement,
                        UserPrivileges = StatementManager.GetUserPrivilegesByStatementId(id),
                        KindergartenName = Kindergarten.Name,
                        UserName = siteUser.FullName
                    };

                    MailCustom.Mail(email, "Заява в електронну чергу #" + model.Statement.Id, GetAnswer(model));

                    return RedirectToAction("Statements", "Kindergarten", new { kindergartenId });
                }
                catch { return RedirectToAction("Statements", "Kindergarten", new { kindergartenId }); }
            };
            return RedirectToAction("Index", "Home");
        }

        #region Help

        private string GetAnswer(StatementListItemViewModel model)
        {
            string result = "<html><head><style> .bold{ font-weight: bold; }</style></head><body><span class='bold'>ПІБ одного з батьків:</span><br>" + model.Statement.SNF + "<br><span class='bold'>ПІБ дитини:</span><br>" + model.Statement.ChildSNF + "<hr><span class='bold'>Серія і номер паспорта одного з батьків:</span><br>" + model.Statement.SeriesNumberPassport + "<br><span class='bold'>Серія і номер свідоцтва про народження дитини:</span><br>" + model.Statement.ChildBirthCertificate + "<hr><span class='bold'>Адреса проживання:</span><br>" + model.Statement.Address + "<br><span class='bold'>Дата народження дитини:</span><br>" + model.Statement.ChildDateOfBirth + "<hr><span class='bold'>Email:</span><br>" + model.Statement.Email + "<br><span class='bold'>Контактний телефон:</span><br>" + model.Statement.PhoneNumber + "<br><span class='bold'>Допоміжний контактний телефон:</span><br>" + model.Statement.AdditionalPhoneNumber + "<hr><span class='bold'>Дошкільний навчальний заклад:</span><br>" + model.KindergartenName + "<br><span class='bold'>Група:</span><br>" + model.Statement.Group + "<br><span class='bold'>Пільги:</span><br>";
            for (int i = 0; i < model.UserPrivileges.Count; i++)
            {
                result += model.UserPrivileges[i] + "<br>";
            }
            return result + "</body></html>";
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

        #endregion
    }
}