using IStudyKindergardens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface IKindergardenManager
    {
        void AddKindergardenClaim(string id, string type, string value);
        void AddKindergardenClaimWithDel(string id, string type, string value);
        void AddKindergarden(AddKindergardenViewModel model, string userId, HttpServerUtilityBase server);
        void AddPreviewPicture(string id, string previewPictureName, HttpServerUtilityBase server);

        void EditKindergarden(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server, EditKindergardenViewModel model);
        void EditKindergardenAddress(string id, string address);

        string GetKindergardenClaimValue(string id, string type);
        string GetPictureUIDById(string id);
        string GetPreviewPictureUIDById(string id);
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

        public void EditKindergardenAddress(string id, string address)
        {
            db.Kindergardens.Where(k => k.Id == id).First().Address = address;
            db.SaveChanges();
        }

        public void AddKindergardenClaim(string id, string type, string value)
        {
            ClaimType claimType;
            try
            {
                claimType = db.ClaimTypes.Where(c => c.Type == type).First();
            }
            catch (Exception)
            {
                claimType = db.ClaimTypes.Add(new ClaimType { Type = type });
            }
            db.KindergardenClaims.Add(new KindergardenClaim { ClaimTypeId = claimType.Id, KindergardenId = id, ClaimValue = value });
            db.SaveChanges();
        }

        public void AddKindergardenClaimWithDel(string id, string type, string value)
        {
            db.KindergardenClaims.RemoveRange(db.KindergardenClaims.Where(kc => kc.KindergardenId == id && kc.ClaimType.Type == type));
            AddKindergardenClaim(id, type, value);
            db.SaveChanges();
        }

        public string GetKindergardenClaimValue(string id, string type)
        {
            string value = String.Empty;
            try
            {
                value = db.KindergardenClaims.Where(kc => kc.KindergardenId == id && kc.ClaimType.Type == type).First().ClaimValue;
            }
            catch (Exception) { }
            return value;
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

        private void DeletePicture(string pictureName, HttpServerUtilityBase server)
        {
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + pictureName));
        }

        public void EditKindergarden(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server, EditKindergardenViewModel model)
        {
            Kindergarden kindergarden = db.Kindergardens.Where(k => k.Id == userId).First();
            kindergarden.Name = model.Name;
            if (model.PictureName != null && model.PictureName != "default")
            {
                KindergardenClaim kindergardenClaim;
                try
                {
                    kindergardenClaim = db.KindergardenClaims.Where(kc => kc.KindergardenId == userId && kc.ClaimType.Type == "Picture").First();
                    string previosClaimValue = kindergardenClaim.ClaimValue;
                    kindergardenClaim.ClaimValue = model.PictureName;
                    System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName), server.MapPath("~/Images/Uploaded/Source/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + previosClaimValue));
                }
                catch (Exception)
                {
                    AddPictureClaim(userId, model.PictureName, server);
                }
            }
            else if(model.PictureName == "default")
            {
                try
                {
                    KindergardenClaim kindergardenClaim = db.KindergardenClaims.Where(kc => kc.KindergardenId == userId && kc.ClaimType.Type == "Picture").First();
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + kindergardenClaim.ClaimValue));
                    db.KindergardenClaims.Remove(kindergardenClaim);
                }
                catch (Exception) { }
            }
            ChangeDescriptionBlocks(descriptionBlocks, userId, server);
        }

        private void ChangeDescriptionBlocks(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server)
        {
            List<DescriptionBlockTextImage> oldBlocks = db.DescriptionBlocksTextImage.Where(db => db.KindergardenId == userId).ToList();
            List<string> picturePaths = new List<string> { };
            for (int i = 0; i < oldBlocks.Count; i++)
            {
                picturePaths.Add(oldBlocks[i].Image);
            }
            db.DescriptionBlocks.RemoveRange(db.DescriptionBlocks.Where(db => db.KindergardenId == userId));
            for (int i = 0; i < descriptionBlocks.Count; i++)
            {
                if (descriptionBlocks[i].BlockType == "TextImage")
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
            for (int i = 0; i < descriptionBlocks.Count; i++)
            {
                if (descriptionBlocks[i].BlockType == "TextImage")
                {
                    for (int j = 0; j < picturePaths.Count; j++)
                    {
                        if (descriptionBlocks[i].BlockComponents[0] == picturePaths[j])
                        {
                            picturePaths.Remove(picturePaths[j]);
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < picturePaths.Count; i++)
            {
                DeletePicture(picturePaths[i], server);
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

        public string GetPreviewPictureUIDById(string id)
        {
            return db.KindergardenClaims.Where(kg => kg.KindergardenId == id && kg.ClaimType.Type == "PreviewPicture").First().ClaimValue;
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
            MovePicture(pictureName, server);
            db.SaveChanges();
            //System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + pictureName), server.MapPath("~/Images/Uploaded/Source/" + pictureName));
            //System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + pictureName));
        }

        public void AddPreviewPicture(string id, string previewPictureName, HttpServerUtilityBase server)
        {
            try
            {
                List<KindergardenClaim> claims = db.KindergardenClaims.Where(kc => kc.KindergardenId == id && kc.ClaimType.Type == "PreviewPicture").ToList();
                for (int i = 0; i < claims.Count; i++)
                {
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + claims[i].ClaimValue));
                }
                db.KindergardenClaims.RemoveRange(claims);
                AddPreviewPictureClaim(id, previewPictureName);
                db.SaveChanges();
            }
            catch (Exception e)
            { }
        }

        private void AddPreviewPictureClaim(string id, string previewPictureName)
        {
            ClaimType claimType;
            try
            {
                claimType = db.ClaimTypes.Where(c => c.Type == "PreviewPicture").First();
            }
            catch (Exception)
            {
                claimType = db.ClaimTypes.Add(new ClaimType { Type = "PreviewPicture" });
            }
            db.KindergardenClaims.Add(new KindergardenClaim { ClaimTypeId = claimType.Id, KindergardenId = id, ClaimValue = previewPictureName });
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