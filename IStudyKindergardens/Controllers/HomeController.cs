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

        public HomeController(IKindergardenManager kindergardenManager, IRatingManager ratingManager, ISiteUserManager siteUserManager)
        {
            _siteUserManager = siteUserManager;
            _kindergardenManager = kindergardenManager;
            _ratingManager = ratingManager;
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
    }
}