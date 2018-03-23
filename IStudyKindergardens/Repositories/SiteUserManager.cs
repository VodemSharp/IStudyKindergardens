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

        bool SwitchAddRemoveKindergardenForSiteUser(string kindergardenId, string userId);

        void EditSiteUser(EditUserViewModel model, string userId, HttpServerUtilityBase Server);

        void DeleteSiteUser(string userId, HttpServerUtilityBase server);

        IEnumerable<SiteUser> GetSiteUsers();
        SiteUser GetSiteUserById(string id);
        string GetPictureUIDById(string id);

        IEnumerable<SiteUser> GetContactUsers(string id);
        void AddContactUser(string id, string contactUserId);
        void RemoveContactUser(string id, string contactUserId);
        List<AddContactListItemViewModel> GetAddContactListViewModel(string id);

        void WriteMessage(string senderUserId, SendMessageViewModel model, int? reMessageId = null);
        ReMessageList GetReMessageList(string userId, int messageId);
        List<MessageUserListItemModel> GetAllSentMessages(string id);
        List<MessageUserListItemModel> GetAllMessages(string id);
        void ReadMessage(int messageId, string userId);
    }

    public class SiteUserManager : IDisposable, ISiteUserManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void RegisterSiteUser(SiteUser siteUser)
        {
            db.SiteUsers.Add(siteUser);
            db.SaveChanges();
        }

        public bool SwitchAddRemoveKindergardenForSiteUser(string kindergardenId, string userId)
        {
            if (db.SiteUserKindergardens.Any(suk => suk.KindergardenId == kindergardenId && suk.SiteUserId == userId))
            {
                db.SiteUserKindergardens.Remove(db.SiteUserKindergardens.Where(suk => suk.KindergardenId == kindergardenId && suk.SiteUserId == userId).First());
                db.SaveChanges();
                return false;
            }
            else
            {
                db.SiteUserKindergardens.Add(new SiteUserKindergarden { SiteUserId = userId, KindergardenId = kindergardenId });
                db.SaveChanges();
                return true;
            }
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
            return db.SiteUsers.ToList();
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
                db.SiteUserClaims.RemoveRange(db.SiteUserClaims.Where(suc => suc.SiteUserId == userId));
                db.SiteUserKindergardens.RemoveRange(db.SiteUserKindergardens.Where(suk => suk.SiteUserId == userId));
                List<Rating> userRatings = db.Ratings.Where(r => r.SiteUserId == userId).ToList();
                for (int i = 0; i < userRatings.Count; i++)
                {
                    for (int j = 0; j < userRatings[i].QuestionRatings.Count; j++)
                    {
                        db.QuestionRatings.Remove(userRatings[i].QuestionRatings.ToList()[j]);
                    }
                }
                db.Ratings.RemoveRange(userRatings);
                if (siteUser != null)
                {
                    db.Users.Remove(siteUser.ApplicationUser);
                    db.SiteUsers.Remove(siteUser);
                    db.SaveChanges();
                }
            }
            catch (Exception) { }
        }

        public IEnumerable<SiteUser> GetContactUsers(string id)
        {
            List<SiteUser> siteUsers = new List<SiteUser>();
            List<SiteUserContact> siteUserContacts = db.SiteUserContacts.Where(suc => suc.SiteUserId == id).ToList();
            Contact tempContact;
            SiteUserContact tempSiteUserContact;
            for (int i = 0; i < siteUserContacts.Count; i++)
            {
                tempSiteUserContact = siteUserContacts[i];
                tempContact = db.Contacts.Where(c => c.Id == tempSiteUserContact.ContactId).First();
                siteUsers.Add(db.SiteUsers.Where(su => su.Id == tempContact.SiteUserId).First());
            }
            return siteUsers;
        }

        public void AddContactUser(string id, string contactUserId)
        {
            Contact contact = db.Contacts.Add(new Contact { SiteUserId = contactUserId });
            db.SiteUserContacts.Add(new SiteUserContact { ContactId = contact.Id, SiteUserId = id });
            db.SaveChanges();
        }

        public void RemoveContactUser(string id, string contactUserId)
        {
            List<SiteUserContact> siteUserContacts = db.SiteUserContacts.Where(suc => suc.SiteUserId == id).ToList();
            Contact contact;
            int tempId;
            for (int i = 0; i < siteUserContacts.Count; i++)
            {
                tempId = siteUserContacts[i].ContactId;
                contact = db.Contacts.Where(c => c.Id == tempId).First();
                if (contact.SiteUserId == contactUserId)
                {
                    db.SiteUserContacts.Remove(db.SiteUserContacts.Where(suc => suc.ContactId == tempId).First());
                    db.Contacts.Remove(contact);
                    db.SaveChanges();
                    return;
                }
            }
        }

        public List<AddContactListItemViewModel> GetAddContactListViewModel(string id)
        {
            List<AddContactListItemViewModel> model = new List<AddContactListItemViewModel>();
            List<SiteUser> allSiteUsers = GetSiteUsers().ToList();
            List<SiteUser> addedSiteUsers = GetContactUsers(id).ToList();
            bool isAdded;
            for (int i = 0; i < allSiteUsers.Count; i++)
            {
                isAdded = false;
                for (int j = 0; j < addedSiteUsers.Count; j++)
                {
                    if (allSiteUsers[i] == addedSiteUsers[j])
                    {
                        isAdded = true;
                        break;
                    }
                }
                model.Add(new AddContactListItemViewModel { SiteUser = allSiteUsers[i], isAdded = isAdded });
            }
            return model;
        }

        public void WriteMessage(string senderUserId, SendMessageViewModel model, int? reMessageId = null)
        {
            Message message = db.Messages.Add(new Message { ApplicationUserId = model.ToUserId, Theme = model.Theme, Text = model.Text, IsRead = false, DateTime = DateTime.Now });
            if (reMessageId != null)
            {
                ReMessage reMessage;
                if (db.ReMessages.Any(rm => rm.Id == reMessageId.Value))
                {
                    reMessage = db.ReMessages.Where(rm => rm.Id == reMessageId.Value).First();
                }
                else
                {
                    reMessage = db.ReMessages.Add(new ReMessage { Id = reMessageId.Value });
                }
                db.MessageReMessages.Add(new MessageReMessage { MessageId = message.Id, ReMessageId = reMessage.Id });
            }
            db.ApplicationUserMessages.Add(new ApplicationUserMessage { MessageId = message.Id, ApplicationUserId = senderUserId });
            db.SaveChanges();
        }

        public List<MessageUserListItemModel> GetAllMessages(string id)
        {
            List<MessageUserListItemModel> model = new List<MessageUserListItemModel> { };
            List<Message> messages = db.Messages.Where(m => m.ApplicationUserId == id).ToList();
            ApplicationUserMessage tempApplicationUserMessage;
            Kindergarden tempKindergarden;
            SiteUser tempSiteUser;
            int tempId;
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                model.Add(new MessageUserListItemModel
                {
                    Theme = messages[i].Theme,
                    Text = messages[i].Text,
                    MessageId = messages[i].Id,
                    IsRead = messages[i].IsRead,
                    DateTime = messages[i].DateTime
                });
                tempId = messages[i].Id;
                tempApplicationUserMessage = db.ApplicationUserMessages.Where(aum => aum.MessageId == tempId).First();
                if (db.Kindergardens.Any(k => k.Id == tempApplicationUserMessage.ApplicationUserId))
                {
                    tempKindergarden = db.Kindergardens.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                    model[model.Count - 1].FromId = tempKindergarden.Id;
                    model[model.Count - 1].From = tempKindergarden.Name;
                    model[model.Count - 1].IsFromUser = false;
                }
                if (db.SiteUsers.Any(su => su.Id == tempApplicationUserMessage.ApplicationUserId))
                {
                    tempSiteUser = db.SiteUsers.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                    model[model.Count - 1].FromId = tempSiteUser.Id;
                    model[model.Count - 1].From = tempSiteUser.FullName;
                    model[model.Count - 1].IsFromUser = true;
                }
            }
            return model;
        }

        public List<MessageUserListItemModel> GetAllSentMessages(string id)
        {
            List<MessageUserListItemModel> model = new List<MessageUserListItemModel> { };
            //List<Message> messages = db.Messages.Where(m => m.ApplicationUserId == id).ToList();
            List<ApplicationUserMessage> applicationUserMessages = db.ApplicationUserMessages.Where(aum => aum.ApplicationUserId == id).ToList();
            SiteUser tempSiteUser;
            Kindergarden tempKindergarden;
            Message tempMessage;
            int tempId;
            string stringTempId;
            for (int i = applicationUserMessages.Count - 1; i >= 0; i--)
            {
                tempId = applicationUserMessages[i].MessageId;
                tempMessage = db.Messages.Where(m => m.Id == tempId).First();

                model.Add(new MessageUserListItemModel
                {
                    Theme = tempMessage.Theme,
                    Text = tempMessage.Text,
                    MessageId = tempMessage.Id,
                    IsRead = tempMessage.IsRead,
                    DateTime = tempMessage.DateTime
                });

                stringTempId = tempMessage.ApplicationUserId;
                if (db.Kindergardens.Any(k => k.Id == stringTempId))
                {
                    tempKindergarden = db.Kindergardens.Where(su => su.Id == stringTempId).First();
                    model[model.Count - 1].FromId = tempKindergarden.Id;
                    model[model.Count - 1].From = tempKindergarden.Name;
                    model[model.Count - 1].IsFromUser = false;
                }
                if (db.SiteUsers.Any(su => su.Id == stringTempId))
                {
                    tempSiteUser = db.SiteUsers.Where(su => su.Id == stringTempId).First();
                    model[model.Count - 1].FromId = tempSiteUser.Id;
                    model[model.Count - 1].From = tempSiteUser.FullName;
                    model[model.Count - 1].IsFromUser = true;
                }
            }
            return model;
        }

        public MessageUserListItemModel GetMessage(string userId, int messageId)
        {
            if (db.ApplicationUserMessages.Any(aum => aum.MessageId == messageId && aum.ApplicationUserId == userId))
            {
                Message message = db.Messages.Where(m => m.Id == messageId).First();
                MessageUserListItemModel model = new MessageUserListItemModel
                {
                    Theme = message.Theme,
                    Text = message.Text,
                    MessageId = message.Id,
                    DateTime = message.DateTime
                };

                ApplicationUserMessage tempApplicationUserMessage = db.ApplicationUserMessages.Where(aum => aum.MessageId == message.Id).First();
                if (db.Kindergardens.Any(k => k.Id == tempApplicationUserMessage.ApplicationUserId))
                {
                    Kindergarden tempKindergarden = db.Kindergardens.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                    model.FromId = tempKindergarden.Id;
                    model.From = tempKindergarden.Name;
                    model.IsFromUser = false;
                }
                if (db.SiteUsers.Any(su => su.Id == tempApplicationUserMessage.ApplicationUserId))
                {
                    SiteUser tempSiteUser = db.SiteUsers.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                    model.FromId = tempSiteUser.Id;
                    model.From = tempSiteUser.FullName;
                    model.IsFromUser = true;
                }
                return model;
            }
            return null;
        }

        private Message GetReMessage(int messageId)
        {
            try
            {
                int reMessageId = db.MessageReMessages.Where(mrm => mrm.MessageId == messageId).First().ReMessageId;
                return db.Messages.Where(m => m.Id == reMessageId).First();
            }
            catch
            {
                return null;
            }
        }

        private MessageUserIdPair GetFromUserMessage(int messageId)
        {
            MessageUserIdPair result = new MessageUserIdPair();
            ApplicationUserMessage tempApplicationUserMessage = db.ApplicationUserMessages.Where(aum => aum.MessageId == messageId).First();
            if (db.Kindergardens.Any(k => k.Id == tempApplicationUserMessage.ApplicationUserId))
            {
                Kindergarden tempKindergarden = db.Kindergardens.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                result.UserId = tempKindergarden.Id;
                result.User = tempKindergarden.Name;
                result.IsUser = false;
            }
            if (db.SiteUsers.Any(su => su.Id == tempApplicationUserMessage.ApplicationUserId))
            {
                SiteUser tempSiteUser = db.SiteUsers.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                result.UserId = tempSiteUser.Id;
                result.User = tempSiteUser.FullName;
                result.IsUser = true;
            }
            return result;
        }

        private bool IsUser(string userId)
        {
            if (db.SiteUsers.Any(su => su.Id == userId))
            {
                return true;
            }
            return false;
        }

        public ReMessageList GetReMessageList(string userId, int messageId)
        {
            if (db.ApplicationUserMessages.Any(aum => aum.MessageId == messageId))
            {
                //ПЕРЕВІРКА!
                Message message = db.Messages.Where(m => m.Id == messageId).First();
                ReMessageList model = new ReMessageList
                {
                    Theme = message.Theme
                };

                MessageUserIdPair tempMessageUserIdPair = GetFromUserMessage(messageId);
                model.ReMessageId = messageId;

                if (message.ApplicationUserId == userId)
                {
                    model.ReceiverId = tempMessageUserIdPair.UserId;
                    model.IsUser = tempMessageUserIdPair.IsUser;
                }
                else
                {
                    model.ReceiverId = message.ApplicationUserId;
                    model.IsUser = IsUser(message.ApplicationUserId);
                }

                model.ReMessages = new List<ReMessageItem> { };

                Message tempMessage = message;
                for (; ; )
                {
                    model.ReMessages.Add(new ReMessageItem { Sender = tempMessageUserIdPair.User, SenderId = tempMessageUserIdPair.UserId, IsUser = tempMessageUserIdPair.IsUser, Text = tempMessage.Text, DateTime = tempMessage.DateTime });
                    tempMessage = GetReMessage(tempMessage.Id);
                    if (tempMessage != null)
                    {
                        tempMessageUserIdPair = GetFromUserMessage(tempMessage.Id);
                    }
                    else
                    {
                        break;
                    }
                }
                //model.ReceiverId = tempMessageUserIdPair.UserId;
                //model.IsUser = tempMessageUserIdPair.IsUser;
                return model;
            }
            return null;
        }

        public void ReadMessage(int messageId, string userId)
        {
            if (db.Messages.Any(m => m.Id == messageId && m.ApplicationUserId == userId))
            {
                db.Messages.Where(m => m.Id == messageId && m.ApplicationUserId == userId).First().IsRead = true;
                db.SaveChanges();
            }
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