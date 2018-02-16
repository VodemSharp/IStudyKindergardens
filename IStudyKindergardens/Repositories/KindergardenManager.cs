using IStudyKindergardens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface IKindergardenManager
    { 
        void AddKindergarden(AddKindergardenViewModel model, string userId, HttpServerUtilityBase server);

        string GetPictureUIDById(string id);
        Kindergarden GetKindergardenById(string id);
        IEnumerable<Kindergarden> GetKindergardens();
    }

    public class KindergardenManager : IDisposable, IKindergardenManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<Kindergarden> GetKindergardens()
        {
            return db.Kindergardens.ToList<Kindergarden>();
        }

        public Kindergarden GetKindergardenById(string id)
        {
            Kindergarden kindergarden = null;
            try
            {
                kindergarden = db.Kindergardens.Where(su => su.Id == id).First();
            }
            catch (Exception) { }
            return kindergarden;
        }

        public void AddKindergarden(AddKindergardenViewModel model, string userId, HttpServerUtilityBase server)
        {
            Kindergarden kindergarden = new Kindergarden { Name = model.Name, Address = model.Address, Id = userId };
            db.Kindergardens.Add(kindergarden);
            if (model.PictureName != null)
            {
                AddPictureClaim(userId, model.PictureName, server);
            }
            db.SaveChanges();
        }

        public string GetPictureUIDById(string id)
        {
            return db.KindergardenClaims.Where(kg => kg.KindergardenId == id && kg.ClaimType.Type == "Picture").First().ClaimValue;
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
            db.KindergardenClaims.Add(new KindergardenClaim { ClaimTypeId = claimType.Id, KindergardenId = id, ClaimValue = pictureName });
            System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + pictureName), server.MapPath("~/Images/Uploaded/Source/" + pictureName));
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + pictureName));
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