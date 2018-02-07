using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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