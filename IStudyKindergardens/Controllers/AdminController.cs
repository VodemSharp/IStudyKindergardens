using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class AdminController : Controller
    {
        private IDataRepository dataRepository;

        public AdminController(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Users
        public ActionResult Users()
        {
            return View(dataRepository.GetSiteUsers());
        }

        // GET: Admin/AddUser
        public ActionResult AddUser()
        {
            return View();
        }

        // POST: Admin/AddUser
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(RegisterViewModel model, bool isAdministration)
        {
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
                    dataRepository.AddTempPicture(fileName);
                    string path = Server.MapPath("~/Images/Uploaded/Temp/" + fileName);
                    upload.SaveAs(path);
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
                    Session["picture"] = fileName;
                    return Json(fileName);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult DeletePicture()
        {
            try
            {
                System.IO.File.Delete(Server.MapPath("~/Images/Uploaded/Temp/") + Session["picture"].ToString());
                Session["picture"] = null;
                return Json(true);
            }
            catch(Exception)
            {
                return Json(false);
            }
        }

            // GET: Admin/Kindergardens
            public ActionResult Kindergardens()
        {
            return View();
        }

        // GET: Admin/Comments
        public ActionResult Comments()
        {
            return View();
        }
    }
}