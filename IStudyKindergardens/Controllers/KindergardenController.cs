using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using Microsoft.AspNet.Identity;
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
            if (User.Identity.IsAuthenticated && (User.IsInRole("User") || User.IsInRole("Administrator") || User.IsInRole("Moderator") || User.IsInRole("Admin")))
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
                    ViewBag.Blocks = _kindergardenManager.GetDescriptionBlocksById(id);
                    return View(kindergarden);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
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
                    List<DescriptionBlock> descriptionBlocks = _kindergardenManager.GetDescriptionBlocksById(id);
                    EditKindergardenViewModel model = new EditKindergardenViewModel { Name = kindergarden.Name, Address = kindergarden.Address, Email = kindergarden.Email, DescriptionBlocks = descriptionBlocks };
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
        public ActionResult Edit(string id, EditKindergardenViewModel model, string content)
        {
            List<DescriptionBlock> descriptionBlocks = new List<DescriptionBlock> { };
            List<string> blocks = content.Split(new char[] { ':' }).ToList();
            List<string> temp;
            for (int i = 0; i < blocks.Count; i++)
            {
                temp = blocks[i].Split(new char[] { '|' }).ToList();
                switch (temp[0])
                {
                    case "text":
                        descriptionBlocks.Add(new DescriptionBlockText { KindergardenId = id, Header = temp[1], Body = temp[2]});
                        break;
                    case "text-image":
                        descriptionBlocks.Add(new DescriptionBlockTextImage { KindergardenId = id, Image = temp[1], Header = temp[2], Body = temp[3] });
                        break;
                }
            }
            _kindergardenManager.ChangeDescriptionBlocks(descriptionBlocks, id, Server);
            return RedirectToAction("KindergardenProfile", "Kindergarden", new { id = id });
        }
    }
}