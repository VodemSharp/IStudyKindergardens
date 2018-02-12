using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class UserController : Controller
    {
        private IDataRepository dataRepository;

        public UserController(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Number(string id)
        {
            try
            {
                SiteUser siteUser = dataRepository.GetSiteUserById(id);
                ViewBag.PhoneNumber = siteUser.ApplicationUser.PhoneNumber.Substring(4);
                try
                {
                    string PictureUID = dataRepository.GetPictureUIDById(id);
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
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}