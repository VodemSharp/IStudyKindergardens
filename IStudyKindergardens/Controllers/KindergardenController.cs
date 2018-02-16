using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class KindergardenController : Controller
    {
        private readonly IKindergardenManager _kindergardenManager;

        public KindergardenController(IKindergardenManager kindergardenManager)
        {
            _kindergardenManager = kindergardenManager;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Kindergarden/{id}")]
        public ActionResult KindergardenProfile(string id)
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Admin")))
            {
                try
                {
                    Kindergarden kindergarden = _kindergardenManager.GetKindergardenById(id);
                    if (kindergarden == null)
                    {
                        throw new Exception();
                    }
                    try
                    {
                        string PictureUID = _kindergardenManager.GetPictureUIDById(id);
                        if (PictureUID == null)
                        {
                            throw new Exception();
                        }
                        ViewBag.Picture = "/Images/Uploaded/Source/" + PictureUID;
                    }
                    catch (Exception)
                    {
                        ViewBag.Picture = "/Images/Default/anonymKindergarden.jpg";
                    }
                    return View(kindergarden);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}