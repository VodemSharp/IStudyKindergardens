﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Controllers
{
    public class KindergardenController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}