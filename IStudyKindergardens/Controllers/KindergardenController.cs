﻿using IStudyKindergardens.HelpClasses;
using IStudyKindergardens.Models;
using IStudyKindergardens.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                    EditKindergardenViewModel model = new EditKindergardenViewModel { Id = id, Name = kindergarden.Name, Address = kindergarden.Address, Email = kindergarden.Email, DescriptionBlocks = descriptionBlocks };
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
        public ActionResult Edit(EditKindergardenViewModel model, string content)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
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
                            descriptionBlocks.Add(new DescriptionBlockText { KindergardenId = model.Id, Header = temp[1], Body = temp[2] });
                            break;
                        case "text-image":
                            descriptionBlocks.Add(new DescriptionBlockTextImage { KindergardenId = model.Id, Image = temp[1], Header = temp[2], Body = temp[3] });
                            break;
                    }
                }
                _kindergardenManager.EditKindergarden(descriptionBlocks, model.Id, Server, model);
                return RedirectToAction("KindergardenProfile", "Kindergarden", new { id = model.Id });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        private void Crop(Image image, Rectangle selection, string fileName)
        {
            Image newImage = CustomImage.Crop(image, selection);
            newImage.Save(Server.MapPath("~/Images/Uploaded/Source/" + fileName));
            image.Dispose();
        }

        [HttpPost]
        public ActionResult ChangeAvatar(string id, string left, string top, string right, string bottom, string src)
        {
            try
            {
                if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
                {
                    int left_int = Convert.ToInt32(left.Split(new char[] { '.' })[0]);
                    int top_int = Convert.ToInt32(top.Split(new char[] { '.' })[0]);
                    int right_int = Convert.ToInt32(right.Split(new char[] { '.' })[0]);
                    int bottom_int = Convert.ToInt32(bottom.Split(new char[] { '.' })[0]);
                    string expansion = src.Split(new char[] { '.' }).Last();
                    string fileName = Guid.NewGuid().ToString() + '.' + expansion;
                    if (right_int > left_int && bottom_int > top_int)
                    {
                        if ((right_int - left_int) > (bottom_int - top_int))
                        {
                            right_int--;
                        }
                        if ((right_int - left_int) < (bottom_int - top_int))
                        {
                            bottom_int--;
                        }
                        if ((right_int - left_int) != (bottom_int - top_int))
                        {
                            throw new Exception();
                        }
                        Crop(Image.FromFile(Server.MapPath("~" + src)), new Rectangle(left_int, top_int, right_int - left_int, bottom_int - top_int), fileName);
                        _kindergardenManager.AddPreviewPicture(id, fileName, Server);
                    }
                    else
                    {
                        throw new Exception();
                    }
                    return View();
                }
            }
            catch (Exception e) { }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangeAddress(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                ChangeAddressViewModel model = new ChangeAddressViewModel { Address = _kindergardenManager.GetKindergardenById(id).Address, AltAddress = _kindergardenManager.GetKindergardenClaimValue(id, "AltAddress") };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ChangeAddress(ChangeAddressViewModel model)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                _kindergardenManager.AddKindergardenClaimWithDel(model.Id, "AltAddress", model.AltAddress);
                _kindergardenManager.EditKindergardenAddress(model.Id, model.Address);
                return RedirectToAction("KindergardenProfile", "Kindergarden", new { id = model.Id });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult EditShortInfo(string id)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                EditShortInfoViewModel model = new EditShortInfoViewModel { Id = id, ShortInfo = _kindergardenManager.GetKindergardenClaimValue(id, "ShortInfo") };
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditShortInfo(EditShortInfoViewModel model)
        {
            if (User.Identity.IsAuthenticated && ((User.IsInRole("Administrator") && (model.Id == User.Identity.GetUserId())) || User.IsInRole("Admin")))
            {
                _kindergardenManager.AddKindergardenClaimWithDel(model.Id, "ShortInfo", model.ShortInfo);
                return RedirectToAction("KindergardenProfile", "Kindergarden", new { id = model.Id });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}