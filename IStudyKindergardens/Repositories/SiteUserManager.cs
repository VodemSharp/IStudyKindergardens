using IStudyKindergardens.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface ISiteUserManager
    {
        void RegisterSiteUser(SiteUser siteUser);
        void AddSiteUser(AddUserViewModel model, string userId, HttpServerUtilityBase Server);

        void EditSiteUser(EditUserViewModel model, string userId, HttpServerUtilityBase Server);

        void DeleteSiteUser(string userId, HttpServerUtilityBase server);

        IEnumerable<SiteUser> GetSiteUsers();
        SiteUser GetSiteUserById(string id);
        string GetPictureUIDById(string id);
    }

    public class SiteUserManager : IDisposable, ISiteUserManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void RegisterSiteUser(SiteUser siteUser)
        {
            db.SiteUsers.Add(siteUser);
            db.SaveChanges();
        }

        private void AddPictureClaim(string id, string pictureName, HttpServerUtilityBase server)
        {
            ClaimType claimType;
            try
            {
                claimType = db.ClaimTypes.Where(c => c.Type == "Picture").First();
            }
            catch (Exception)
            {
                claimType = db.ClaimTypes.Add(new ClaimType { Type = "Picture" });
            }
            db.SiteUserClaims.Add(new SiteUserClaim { ClaimTypeId = claimType.Id, SiteUserId = id, ClaimValue = pictureName });
            System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + pictureName), server.MapPath("~/Images/Uploaded/Source/" + pictureName));
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + pictureName));
        }

        public void AddSiteUser(AddUserViewModel model, string userId, HttpServerUtilityBase server)
        {
            SiteUser siteUser = new SiteUser { Name = model.Name, Surname = model.Surname, FathersName = model.FathersName, DateOfBirth = model.DateOfBirth, Id = userId };
            db.SiteUsers.Add(siteUser);
            if (model.PictureName != null)
            {
                AddPictureClaim(userId, model.PictureName, server);
            }
            db.SaveChanges();
        }

        public IEnumerable<SiteUser> GetSiteUsers()
        {
            return db.SiteUsers.ToList<SiteUser>();
        }

        public SiteUser GetSiteUserById(string id)
        {
            SiteUser siteUser = null;
            try
            {
                siteUser = db.SiteUsers.Where(su => su.Id == id).First();
            }
            catch (Exception) { }
            return siteUser;
        }

        public string GetPictureUIDById(string id)
        {
            return db.SiteUserClaims.Where(suc => suc.SiteUserId == id && suc.ClaimType.Type == "Picture").First().ClaimValue;
        }

        public void EditSiteUser(EditUserViewModel model, string userId, HttpServerUtilityBase server)
        {
            SiteUser siteUser = db.SiteUsers.Where(su => su.Id == userId).First();
            siteUser.Name = model.Name;
            siteUser.Surname = model.Surname;
            siteUser.FathersName = model.FathersName;
            siteUser.ApplicationUser.Email = model.Email;
            siteUser.ApplicationUser.PhoneNumber = "+38 " + model.PhoneNumber;
            if (model.PictureName != null)
            {
                SiteUserClaim siteUserClaim;
                try
                {
                    siteUserClaim = db.SiteUserClaims.Where(suc => suc.SiteUserId == userId && suc.ClaimType.Type == "Picture").First();
                    string previosClaimValue = siteUserClaim.ClaimValue;
                    siteUserClaim.ClaimValue = model.PictureName;
                    System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName), server.MapPath("~/Images/Uploaded/Source/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + previosClaimValue));
                }
                catch (Exception)
                {
                    AddPictureClaim(siteUser.Id, model.PictureName, server);
                }
            }
            else
            {
                try
                {
                    SiteUserClaim siteUserClaim = db.SiteUserClaims.Where(suc => suc.SiteUserId == userId && suc.ClaimType.Type == "Picture").First();
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + siteUserClaim.ClaimValue));
                    db.SiteUserClaims.Remove(siteUserClaim);
                }
                catch (Exception) { }
            }
            siteUser.DateOfBirth = model.DateOfBirth;
            db.SaveChanges();
        }

        public void DeleteSiteUser(string userId, HttpServerUtilityBase server)
        {
            try
            {
                SiteUserClaim siteUserClaim = db.SiteUserClaims.Where(suc => suc.SiteUserId == userId && suc.ClaimType.Type == "Picture").First();
                System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + siteUserClaim.ClaimValue));
                db.SiteUserClaims.Remove(siteUserClaim);
            }
            catch (Exception) { }
            try
            {
                SiteUser siteUser = db.SiteUsers.Include("ApplicationUser").Where(su => su.Id == userId).First();
                if (siteUser != null)
                {
                    db.Users.Remove(siteUser.ApplicationUser);
                    db.SiteUsers.Remove(siteUser);
                    db.SaveChanges();
                }
            }
            catch (Exception) { }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}