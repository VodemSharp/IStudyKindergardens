using IStudyKindergardens.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface IDataRepository
    {
        void AddTempPicture(string name);

        void RegisterSiteUser(SiteUser siteUser);
        void AddSiteUser(AddUserViewModel model, string userId, HttpServerUtilityBase Server = null);

        IEnumerable<SiteUser> GetSiteUsers();
        SiteUser GetSiteUserById(string id);
        string GetPictureUIDById(string id);
    }

    public class DataRepository : IDisposable, IDataRepository
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void RegisterSiteUser(SiteUser siteUser)
        {
            db.SiteUsers.Add(siteUser);
            db.SaveChanges();
        }

        public void AddSiteUser(AddUserViewModel model, string userId, HttpServerUtilityBase Server = null)
        {
            SiteUser siteUser = new SiteUser { Name = model.Name, Surname = model.Surname, FathersName = model.FathersName, DateOfBirth = model.DateOfBirth, Id = userId };
            db.SiteUsers.Add(siteUser);
            if (model.PictureName != null)
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
                db.SiteUserClaims.Add(new SiteUserClaim { ClaimTypeId = claimType.Id, SiteUserId = siteUser.Id, ClaimValue = model.PictureName });
                System.IO.File.Copy(Server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName), Server.MapPath("~/Images/Uploaded/Source/" + model.PictureName));
                System.IO.File.Delete(Server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName));
            }
            db.SaveChanges();
        }

        public void AddTempPicture(string name)
        {
            db.TempPictures.Add(new TempPicture { Name = name, Time = DateTime.Now });
            db.SaveChanges();
        }

        public IEnumerable<SiteUser> GetSiteUsers()
        {
            return db.SiteUsers.ToList<SiteUser>();
        }

        public SiteUser GetSiteUserById(string id)
        {
            return db.SiteUsers.Where(su => su.Id == id).First();
        }

        public string GetPictureUIDById(string id)
        {
            return db.SiteUserClaims.Where(suc => suc.SiteUserId == id).First().ClaimValue;
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