using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
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
        private readonly IKindergardenManager _kindergardenManager;
        private readonly IRatingManager _ratingManager;

        public HomeController(IKindergardenManager kindergardenManager, IRatingManager ratingManager)
        {
            _kindergardenManager = kindergardenManager;
            _ratingManager = ratingManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            KindergardenListViewModel model = new KindergardenListViewModel { Kindergardens = _kindergardenManager.GetKindergardens().ToList() };
            model.Addresses = new List<string> { };
            model.PreviewPictures = new List<string> { };
            model.ShortInfo = new List<string> { };
            model.Ratings = new List<string> { };
            for (int i = 0; i < model.Kindergardens.Count; i++)
            {
                model.Addresses.Add(_kindergardenManager.GetKindergardenClaimValue(model.Kindergardens[i].Id, "AltAddress"));
                model.PreviewPictures.Add(_kindergardenManager.GetKindergardenClaimValue(model.Kindergardens[i].Id, "PreviewPicture"));
                model.ShortInfo.Add(_kindergardenManager.GetKindergardenClaimValue(model.Kindergardens[i].Id, "ShortInfo"));
                model.Ratings.Add(_ratingManager.CalculateRating(model.Kindergardens[i].Id).ToString());
            }
            return View(model);
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
    }
}