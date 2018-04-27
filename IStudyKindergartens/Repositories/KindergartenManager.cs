using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Kindergartens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Repositories
{
    public interface IKindergartenManager
    {
        void AddKindergartenClaim(string id, string type, string value);
        void AddKindergartenClaimWithDel(string id, string type, string value);
        void AddKindergarten(AddKindergartenViewModel model, string userId, HttpServerUtilityBase server);
        void AddPreviewPicture(string id, string previewPictureName, HttpServerUtilityBase server);

        void EditKindergarten(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server, EditKindergartenViewModel model);
        void EditKindergartenAddress(string id, string address);

        void DeleteKindergarten(string id, HttpServerUtilityBase server);

        string GetKindergartenClaimValue(string id, string type);
        string GetPictureUIDById(string id);
        string GetPreviewPictureUIDById(string id);
        Kindergarten GetKindergartenById(string id);
        List<DescriptionBlock> GetDescriptionBlocksById(string id);
        IEnumerable<Kindergarten> GetKindergartens();

        List<Kindergarten> GetKindergartensForSiteUser(string userId);
        List<KindergartenListItemViewModel> GetFormatKindergartenListViewModel(bool isOnlySelected, string userId, string search = null, string searchBy = null, string sortBy = null, int skip = 0, int take = 0);
    }

    public class KindergartenManager : IDisposable, IKindergartenManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region AddSomething

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
            db.KindergartenClaims.Add(new KindergartenClaim { ClaimTypeId = claimType.Id, KindergartenId = id, ClaimValue = pictureName });
            MovePicture(pictureName, server);
            db.SaveChanges();
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
            db.KindergartenClaims.Add(new KindergartenClaim { ClaimTypeId = claimType.Id, KindergartenId = id, ClaimValue = previewPictureName });
        }

        public void AddKindergartenClaim(string id, string type, string value)
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
            db.KindergartenClaims.Add(new KindergartenClaim { ClaimTypeId = claimType.Id, KindergartenId = id, ClaimValue = value });
            db.SaveChanges();
        }

        public void AddKindergartenClaimWithDel(string id, string type, string value)
        {
            db.KindergartenClaims.RemoveRange(db.KindergartenClaims.Where(kc => kc.KindergartenId == id && kc.ClaimType.Type == type));
            AddKindergartenClaim(id, type, value);
            db.SaveChanges();
        }

        public void AddKindergarten(AddKindergartenViewModel model, string userId, HttpServerUtilityBase server)
        {
            Kindergarten Kindergarten = new Kindergarten { Name = model.Name, Address = model.Address, Id = userId, ActualRating = -1 };
            db.Kindergartens.Add(Kindergarten);
            if (model.PictureName != null)
            {
                AddPictureClaim(userId, model.PictureName, server);
            }
            db.SaveChanges();
        }

        public void AddPreviewPicture(string id, string previewPictureName, HttpServerUtilityBase server)
        {
            try
            {
                List<KindergartenClaim> claims = db.KindergartenClaims.Where(kc => kc.KindergartenId == id && kc.ClaimType.Type == "PreviewPicture").ToList();
                for (int i = 0; i < claims.Count; i++)
                {
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + claims[i].ClaimValue));
                }
                db.KindergartenClaims.RemoveRange(claims);
                AddPreviewPictureClaim(id, previewPictureName);
                db.SaveChanges();
            }
            catch (Exception)
            { }
        }

        #endregion

        #region EditSomething

        public void EditKindergarten(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server, EditKindergartenViewModel model)
        {
            Kindergarten Kindergarten = db.Kindergartens.Where(k => k.Id == userId).First();
            Kindergarten.Name = model.Name;
            if (model.PictureName != null && model.PictureName != "default")
            {
                KindergartenClaim KindergartenClaim;
                try
                {
                    KindergartenClaim = db.KindergartenClaims.Where(kc => kc.KindergartenId == userId && kc.ClaimType.Type == "Picture").First();
                    string previosClaimValue = KindergartenClaim.ClaimValue;
                    KindergartenClaim.ClaimValue = model.PictureName;
                    System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName), server.MapPath("~/Images/Uploaded/Source/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + model.PictureName));
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + previosClaimValue));
                }
                catch (Exception)
                {
                    AddPictureClaim(userId, model.PictureName, server);
                }
            }
            else if (model.PictureName == "default")
            {
                try
                {
                    KindergartenClaim KindergartenClaim = db.KindergartenClaims.Where(kc => kc.KindergartenId == userId && kc.ClaimType.Type == "Picture").First();
                    System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + KindergartenClaim.ClaimValue));
                    db.KindergartenClaims.Remove(KindergartenClaim);
                }
                catch (Exception) { }
            }
            ChangeDescriptionBlocks(descriptionBlocks, userId, server);
        }

        public void EditKindergartenAddress(string id, string address)
        {
            db.Kindergartens.Where(k => k.Id == id).First().Address = address;
            db.SaveChanges();
        }

        #endregion

        #region DeleteSomething

        private void DeletePicture(string pictureName, HttpServerUtilityBase server)
        {
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + pictureName));
        }

        public void DeleteKindergarten(string id, HttpServerUtilityBase server)
        {
            try
            {
                KindergartenClaim KindergartenClaim = db.KindergartenClaims.Where(suc => suc.KindergartenId == id && suc.ClaimType.Type == "Picture").First();
                System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + KindergartenClaim.ClaimValue));
                db.KindergartenClaims.Remove(KindergartenClaim);
            }
            catch (Exception) { }
            try
            {
                KindergartenClaim KindergartenClaim = db.KindergartenClaims.Where(suc => suc.KindergartenId == id && suc.ClaimType.Type == "PreviewPicture").First();
                System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Source/" + KindergartenClaim.ClaimValue));
                db.KindergartenClaims.Remove(KindergartenClaim);
            }
            catch (Exception) { }
            db.KindergartenClaims.RemoveRange(db.KindergartenClaims.Where(kc => kc.KindergartenId == id));
            List<Rating> KindergartenRatings = db.Ratings.Where(r => r.KindergartenId == id).ToList();
            for (int i = 0; i < KindergartenRatings.Count; i++)
            {
                for (int j = 0; j < KindergartenRatings[i].QuestionRatings.Count; j++)
                {
                    db.QuestionRatings.Remove(KindergartenRatings[i].QuestionRatings.ToList()[j]);
                }
            }
            db.Ratings.RemoveRange(KindergartenRatings);
            db.DescriptionBlocks.RemoveRange(db.DescriptionBlocks.Where(db => db.KindergartenId == id));
            db.SiteUserKindergartens.RemoveRange(db.SiteUserKindergartens.Where(suk => suk.KindergartenId == id));
            db.Statements.RemoveRange(db.Statements.Where(s => s.KindergartenId == id));

            Kindergarten Kindergarten = db.Kindergartens.Include("ApplicationUser").Where(k => k.Id == id).First();
            if (Kindergarten != null)
            {
                db.Users.Remove(Kindergarten.ApplicationUser);
                db.Kindergartens.Remove(Kindergarten);
                db.SaveChanges();
            }
        }

        #endregion

        #region GetSomething

        public IEnumerable<Kindergarten> GetKindergartens()
        {
            return db.Kindergartens.ToList<Kindergarten>();
        }

        public string GetKindergartenClaimValue(string id, string type)
        {
            string value = String.Empty;
            try
            {
                value = db.KindergartenClaims.Where(kc => kc.KindergartenId == id && kc.ClaimType.Type == type).First().ClaimValue;
            }
            catch (Exception) { }
            return value;
        }

        public Kindergarten GetKindergartenById(string id)
        {
            Kindergarten Kindergarten = null;
            try
            {
                Kindergarten = db.Kindergartens.Where(su => su.Id == id).First();
            }
            catch (Exception) { }
            return Kindergarten;
        }

        public string GetPictureUIDById(string id)
        {
            return db.KindergartenClaims.Where(kg => kg.KindergartenId == id && kg.ClaimType.Type == "Picture").First().ClaimValue;
        }

        public string GetPreviewPictureUIDById(string id)
        {
            return db.KindergartenClaims.Where(kg => kg.KindergartenId == id && kg.ClaimType.Type == "PreviewPicture").First().ClaimValue;
        }

        public List<DescriptionBlock> GetDescriptionBlocksById(string id)
        {
            return db.DescriptionBlocks.Where(db => db.KindergartenId == id).ToList();
        }

        public List<Kindergarten> GetKindergartensForSiteUser(string userId)
        {
            List<Kindergarten> Kindergartens = new List<Kindergarten> { };
            List<SiteUserKindergarten> siteUserKindergartens = db.SiteUserKindergartens.Where(suk => suk.SiteUserId == userId).ToList();
            string tempId;
            for (int i = 0; i < siteUserKindergartens.Count; i++)
            {
                tempId = siteUserKindergartens[i].KindergartenId;
                Kindergartens.Add(db.Kindergartens.Where(k => k.Id == tempId).First());
            }
            return Kindergartens;
        }

        public List<KindergartenListItemViewModel> GetFormatKindergartenListViewModel(bool isOnlySelected, string userId, string search = null, string searchBy = null, string sortBy = null, int skip = 0, int take = 0)
        {
            List<KindergartenListItemViewModel> model = new List<KindergartenListItemViewModel> { };
            List<Kindergarten> Kindergartens;

            search = search ?? "";
            searchBy = searchBy ?? "Name";
            sortBy = sortBy ?? "Desc";
            if (isOnlySelected)
            {
                Kindergartens = GetKindergartensForSiteUser(userId);
            }
            else
            {
                Kindergartens = db.Kindergartens.ToList();
            }
            if (searchBy == "Address")
            {
                Kindergartens = Kindergartens.Where(k => k.Address.Contains(search) || CheckAltAddress(k.Id, search)).ToList();
            }
            else
            {
                Kindergartens = Kindergartens.Where(k => k.Name.Contains(search)).ToList();
            }
            if (sortBy == "Asc")
            {
                Kindergartens = Kindergartens.OrderBy(k => k.ActualRating).ThenBy(k => k.Name).ToList();
            }
            else
            {
                Kindergartens = Kindergartens.OrderByDescending(k => k.ActualRating).ThenBy(k => k.Name).ToList();
            }
            if (skip != -1 || take != -1)
            {
                Kindergartens = Kindergartens.Skip(skip).Take(take).ToList();
            }
            string tempRating;
            for (int i = 0; i < Kindergartens.Count; i++)
            {
                tempRating = Kindergartens[i].ActualRating.ToString();
                model.Add(new KindergartenListItemViewModel
                {
                    Kindergarten = Kindergartens[i],
                    Address = GetKindergartenClaimValue(Kindergartens[i].Id, "AltAddress"),
                    PreviewPicture = GetKindergartenClaimValue(Kindergartens[i].Id, "PreviewPicture"),
                    ShortInfo = GetKindergartenClaimValue(Kindergartens[i].Id, "ShortInfo"),
                    Rating = (tempRating == "-1") ? "-" : tempRating,
                });
                if (isOnlySelected)
                {
                    model[model.Count - 1].IsSelected = true;
                }
                else
                {
                    model[model.Count - 1].IsSelected = IsKindergartenSelected(userId, Kindergartens[i].Id);
                }
            }
            return model;
        }

        #endregion

        #region Help

        private void MovePicture(string pictureName, HttpServerUtilityBase server)
        {
            System.IO.File.Copy(server.MapPath("~/Images/Uploaded/Temp/" + pictureName), server.MapPath("~/Images/Uploaded/Source/" + pictureName));
            System.IO.File.Delete(server.MapPath("~/Images/Uploaded/Temp/" + pictureName));
        }

        private void ChangeDescriptionBlocks(List<DescriptionBlock> descriptionBlocks, string userId, HttpServerUtilityBase server)
        {
            List<DescriptionBlockTextImage> oldBlocks = db.DescriptionBlocksTextImage.Where(db => db.KindergartenId == userId).ToList();
            List<string> picturePaths = new List<string> { };
            for (int i = 0; i < oldBlocks.Count; i++)
            {
                picturePaths.Add(oldBlocks[i].Image);
            }
            db.DescriptionBlocks.RemoveRange(db.DescriptionBlocks.Where(db => db.KindergartenId == userId));
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

        private bool CheckAltAddress(string KindergartenId, string search)
        {
            return GetKindergartenClaimValue(KindergartenId, "AltAddress").Contains(search);
        }

        private bool IsKindergartenSelected(string userId, string KindergartenId)
        {
            return db.SiteUserKindergartens.Any(suk => suk.KindergartenId == KindergartenId && suk.SiteUserId == userId);
        }
        
        #endregion

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