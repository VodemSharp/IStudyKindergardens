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

        void ChangeDescriptionBlocks(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server);

        string GetPictureUIDById(string id);
        Kindergarden GetKindergardenById(string id);
        List<DescriptionBlock> GetDescriptionBlocksById(string id);
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

        private void MovePicture(string pictureName, HttpServerUtilityBase server)
        {
            System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + pictureName), server.MapPath("~/Images/Uploaded/Source/" + pictureName));
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + pictureName));
        }

        public void ChangeDescriptionBlocks(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server)
        {
            db.DescriptionBlocks.RemoveRange(db.DescriptionBlocks.Where(db => db.KindergardenId == userId));
            for(int i = 0; i < descriptionBlocks.Count; i++)
            {
                if(descriptionBlocks[i].BlockType == "TextImage")
                {
                    string picture = descriptionBlocks[i].BlockComponents[0];
                    if (picture.Substring(0, 6) == "/Temp/")
                    {
                        string temp = picture.Substring(6, picture.Length - 6);
                        MovePicture(temp, server);
                        descriptionBlocks[i].BlockComponents = new List<string> { temp, null, null };
                    }
                    else if (picture.Substring(0, 8) == "/Source/")
                    {
                        string temp = picture.Substring(8, picture.Length - 8);
                        descriptionBlocks[i].BlockComponents = new List<string> { temp, null, null };
                    }
                }
            }
            db.DescriptionBlocks.AddRange(descriptionBlocks);
            db.SaveChanges();
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

        public List<DescriptionBlock> GetDescriptionBlocksById(string id)
        {
            return db.DescriptionBlocks.Where(db => db.KindergardenId == id).ToList();
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