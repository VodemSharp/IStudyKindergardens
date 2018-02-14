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

        [Route("User/{id}")]
        public ActionResult UserProfile(string id)
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
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                SiteUser siteUser = dataRepository.GetSiteUserById(id);
                string picture;
                string phoneNumber = siteUser.ApplicationUser.PhoneNumber.Substring(4);
                try
                {
                    string PictureUID = dataRepository.GetPictureUIDById(id);
                    if (PictureUID == null)
                    {
                        throw new Exception();
                    }
                    picture = "/Images/Uploaded/Source/" + PictureUID;
                }
                catch (Exception)
                {
                    picture = null;
                }

                return View(new EditUserViewModel { PictureName = picture, Surname = siteUser.Surname, Name = siteUser.Name, FathersName = siteUser.FathersName, Email = siteUser.ApplicationUser.Email, PhoneNumber = phoneNumber, DateOfBirth = siteUser.DateOfBirth });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(EditUserViewModel model, string id)
        {
            try
            {
                dataRepository.EditSiteUser(model, id, Server);
            }
            catch (Exception) { }
            return RedirectToAction("Index", "Home");
        }
    }
}