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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
            }
            catch (Exception) { }
            return Json(true);
        }
    }
}