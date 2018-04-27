using IStudyKindergartens.Models;
using IStudyKindergartens.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Repositories
{
    public interface IMessageManager
    {
        void WriteMessage(string senderUserId, SendMessageViewModel model, int? reMessageId = null);
        void WriteToMessage(string senderUserId, SendMessageToViewModel model);

        int GetCountUnreadMessages(string id);
        ReMessageList GetReMessageList(string userId, int messageId);
        List<MessageUserListItemModel> GetAllSentMessages(string id);
        List<MessageUserListItemModel> GetAllMessages(string id);

        void ReadMessage(int messageId, string userId);
        void HideMessages(List<int> messageId, string userId);
    }

    public class MessageManager : IDisposable, IMessageManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region WriteSomething

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

        public void WriteToMessage(string senderUserId, SendMessageToViewModel model)
        {
            Message message = db.Messages.Add(new Message { ApplicationUserId = model.ToUserId, Theme = model.Theme, Text = model.Text, IsRead = false, DateTime = DateTime.Now });
            db.ApplicationUserMessages.Add(new ApplicationUserMessage { MessageId = message.Id, ApplicationUserId = senderUserId });
            db.SaveChanges();
        }

        #endregion

        #region GetSomething

        public int GetCountUnreadMessages(string id)
        {
            return db.Messages.Where(m => m.ApplicationUserId == id && !m.IsRead).Count();
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
            if (db.Kindergartens.Any(k => k.Id == tempApplicationUserMessage.ApplicationUserId))
            {
                Kindergarten tempKindergarten = db.Kindergartens.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                result.UserId = tempKindergarten.Id;
                result.User = tempKindergarten.Name;
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

        public List<MessageUserListItemModel> GetAllMessages(string id)
        {
            List<MessageUserListItemModel> model = new List<MessageUserListItemModel> { };
            List<Message> messages = db.Messages.Where(m => m.ApplicationUserId == id).ToList();
            ApplicationUserMessage tempApplicationUserMessage;
            Kindergarten tempKindergarten;
            SiteUser tempSiteUser;
            int tempId;
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                if (!messages[i].IsHiddenForReciver)
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
                    if (db.Kindergartens.Any(k => k.Id == tempApplicationUserMessage.ApplicationUserId))
                    {
                        tempKindergarten = db.Kindergartens.Where(su => su.Id == tempApplicationUserMessage.ApplicationUserId).First();
                        model[model.Count - 1].FromId = tempKindergarten.Id;
                        model[model.Count - 1].From = tempKindergarten.Name;
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
            }
            return model;
        }

        public List<MessageUserListItemModel> GetAllSentMessages(string id)
        {
            List<MessageUserListItemModel> model = new List<MessageUserListItemModel> { };
            //List<Message> messages = db.Messages.Where(m => m.ApplicationUserId == id).ToList();
            List<ApplicationUserMessage> applicationUserMessages = db.ApplicationUserMessages.Where(aum => aum.ApplicationUserId == id).ToList();
            SiteUser tempSiteUser;
            Kindergarten tempKindergarten;
            Message tempMessage;
            int tempId;
            string stringTempId;
            for (int i = applicationUserMessages.Count - 1; i >= 0; i--)
            {
                tempId = applicationUserMessages[i].MessageId;
                tempMessage = db.Messages.Where(m => m.Id == tempId).First();
                if (!tempMessage.IsHiddenForSender)
                {

                    model.Add(new MessageUserListItemModel
                    {
                        Theme = tempMessage.Theme,
                        Text = tempMessage.Text,
                        MessageId = tempMessage.Id,
                        IsRead = tempMessage.IsRead,
                        DateTime = tempMessage.DateTime
                    });

                    stringTempId = tempMessage.ApplicationUserId;
                    if (db.Kindergartens.Any(k => k.Id == stringTempId))
                    {
                        tempKindergarten = db.Kindergartens.Where(su => su.Id == stringTempId).First();
                        model[model.Count - 1].FromId = tempKindergarten.Id;
                        model[model.Count - 1].From = tempKindergarten.Name;
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
            }
            return model;
        }

        public ReMessageList GetReMessageList(string userId, int messageId)
        {
            if (db.ApplicationUserMessages.Any(aum => aum.MessageId == messageId))
            {
                Message message = db.Messages.Where(m => m.Id == messageId).First();
                if (message.ApplicationUserId == userId || db.ApplicationUserMessages.Where(aum => aum.MessageId == messageId).First().ApplicationUserId == userId)
                {
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
                    return model;
                }
            }
            return null;
        }

        #endregion

        #region Help

        private bool IsUser(string userId)
        {
            if (db.SiteUsers.Any(su => su.Id == userId))
            {
                return true;
            }
            return false;
        }

        public void ReadMessage(int messageId, string userId)
        {
            if (db.Messages.Any(m => m.Id == messageId && m.ApplicationUserId == userId))
            {
                db.Messages.Where(m => m.Id == messageId && m.ApplicationUserId == userId).First().IsRead = true;
                db.SaveChanges();
            }
        }

        public void HideMessages(List<int> messageId, string userId)
        {
            int tempId;
            for (int i = 0; i < messageId.Count; i++)
            {
                tempId = messageId[i];
                if (db.Messages.Any(m => m.Id == tempId && m.ApplicationUserId == userId))
                {
                    db.Messages.Where(m => m.Id == tempId && m.ApplicationUserId == userId).First().IsHiddenForReciver = true;
                }
                ApplicationUserMessage applicationUserMessage = db.ApplicationUserMessages.Where(aum => aum.MessageId == tempId).First();
                if (applicationUserMessage.ApplicationUserId == userId)
                {
                    db.Messages.Where(m => m.Id == tempId).First().IsHiddenForSender = true;
                }
            }

            db.SaveChanges();
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