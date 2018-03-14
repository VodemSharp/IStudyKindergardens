using IStudyKindergardens.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Repositories
{
    public interface IStatementManager
    {
        List<Group> GetAllGroups();
        Group GetGroupById(int id);
        void AddGroup(Group group);
        void EditGroup(Group group);
        void RemoveGroup(int id);

        List<Privilege> GetAllPrivileges();
        Privilege GetPrivilegeById(int id);
        void AddPrivilege(Privilege privilege);
        void EditPrivilege(Privilege privilege);
        void RemovePrivilege(int id);

        List<Statement> GetAllStatements();
        Statement GetStatementById(int id);
        void AddStatement(Statement statement);

        void SwitchStatus(int statementId, string status);
        void SwitchIsSelected(int statementId, bool isSelected);
        void SwitchIsRemoved(int statementId, bool isRemoved);

        void ApplyStatement(AddStatementViewModel model, string userId);

        List<StatementListItemViewModel> GetFormatStatementListViewModel(string userId = "none", bool isKindergarden = false, string status = "none", bool isAll = false);
    }

    public class StatementManager : IDisposable, IStatementManager
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Privilege> GetAllPrivileges()
        {
            return db.Privileges.ToList();
        }

        public Privilege GetPrivilegeById(int id)
        {
            try
            {
                return db.Privileges.Where(q => q.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddPrivilege(Privilege privilege)
        {
            db.Privileges.Add(privilege);
            db.UserPrivileges.Add(new UserPrivilege { Value = privilege.Value });
            db.SaveChanges();
        }

        public void EditPrivilege(Privilege privilege)
        {
            Privilege editedPrivilege = GetPrivilegeById(privilege.Id);
            editedPrivilege.Value = privilege.Value;
            db.UserPrivileges.Add(new UserPrivilege { Value = editedPrivilege.Value });
            db.SaveChanges();
        }

        public void RemovePrivilege(int id)
        {
            db.Privileges.Remove(db.Privileges.Where(q => q.Id == id).First());
            List<UserPrivilege> userPrivileges = db.UserPrivileges.ToList();
            for (int i = 0; i < userPrivileges.Count; i++)
            {
                if (!db.UserPrivilegeStatements.Any(ups => ups.UserPrivilegeId == userPrivileges[i].Id))
                {
                    db.UserPrivileges.Remove(userPrivileges[i]);
                }
            }
            db.SaveChanges();
        }

        public List<Group> GetAllGroups()
        {
            return db.Groups.ToList();
        }

        public Group GetGroupById(int id)
        {
            try
            {
                return db.Groups.Where(q => q.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddGroup(Group group)
        {
            db.Groups.Add(group);
            db.SaveChanges();
        }

        public void EditGroup(Group group)
        {
            Group editedGroup = GetGroupById(group.Id);
            editedGroup.Value = group.Value;
            db.SaveChanges();
        }

        public void RemoveGroup(int id)
        {
            db.Groups.Remove(db.Groups.Where(g => g.Id == id).First());
            db.SaveChanges();
        }

        public List<Statement> GetAllStatements()
        {
            return db.Statements.ToList();
        }

        public Statement GetStatementById(int id)
        {
            try
            {
                return db.Statements.Where(q => q.Id == id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ApplyStatement(AddStatementViewModel model, string userId)
        {
            Statement statement = new Statement
            {
                KindergardenId = model.SelectedKindergardenId,
                SiteUserId = userId,
                SNF = model.SNF,
                SeriesNumberPassport = model.SeriesNumberPassport,
                ChildSNF = model.ChildSNF,
                ChildDateOfBirth = model.ChildDateOfBirth,
                ChildBirthCertificate = model.ChildBirthCertificate,
                Group = model.SelectedGroup,
                Address = model.Address,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                Status = "NotConsidered",
                IsSelected = false,
                IsRemoved = false,
                DateTime = DateTime.Now
            };

            db.Statements.Add(statement);
            string tempKey;
            for (int i = 0; i < model.Privileges.Count; i++)
            {
                if (model.Privileges[i].Value)
                {
                    tempKey = model.Privileges[i].Key;
                    db.UserPrivilegeStatements.Add(new UserPrivilegeStatement { StatementId = statement.Id, UserPrivilegeId = db.UserPrivileges.Where(up => up.Value == tempKey).First().Id });
                }
            }
            db.SaveChanges();
        }

        public List<StatementListItemViewModel> GetFormatStatementListViewModel(string userId = "none", bool isKindergarden = false, string status = "none", bool isAll = false)
        {
            List<StatementListItemViewModel> model = new List<StatementListItemViewModel> { };
            List<Statement> statements;
            if (userId == "none")
            {
                if (isKindergarden)
                {
                    statements = db.Statements.OrderBy(s => s.DateTime).ToList();
                }
                else
                {
                    statements = db.Statements.OrderByDescending(s => s.DateTime).ToList();
                }
            }
            else
            {
                if (isKindergarden)
                {
                    statements = db.Statements.Where(s => s.KindergardenId == userId).OrderBy(s => s.DateTime).ToList();
                }
                else
                {
                    statements = db.Statements.Where(s => s.SiteUserId == userId).OrderByDescending(s => s.DateTime).ToList();
                }
            }
            if (status == "Removed")
            {
                statements = statements.Where(s => s.IsRemoved == true).ToList();
            }
            else
            {
                if (!isAll)
                {
                    statements = statements.Where(s => s.IsRemoved == false).ToList();
                }
                switch (status)
                {
                    case "Selected":
                        statements = statements.Where(s => s.IsSelected == true).ToList();
                        break;
                    case "Approved":
                        statements = statements.Where(s => s.Status == "Approved").ToList();
                        break;
                    case "Rejected":
                        statements = statements.Where(s => s.Status == "Rejected").ToList();
                        break;
                    case "NotConsidered":
                        statements = statements.Where(s => s.Status == "NotConsidered").ToList();
                        break;
                }
            }
            List<UserPrivilegeStatement> tempUserPrivilegeStatements = new List<UserPrivilegeStatement> { };
            List<string> tempUserPrivileges = new List<string> { };
            SiteUser tempSiteUser;
            string tempStringId;
            int tempId;

            for (int i = 0; i < statements.Count; i++)
            {
                tempStringId = statements[i].SiteUserId;
                tempSiteUser = db.SiteUsers.Where(su => su.Id == tempStringId).First();
                tempId = statements[i].Id;
                tempUserPrivilegeStatements = db.UserPrivilegeStatements.Where(ups => ups.StatementId == tempId).ToList();
                for (int j = 0; j < tempUserPrivilegeStatements.Count; j++)
                {
                    tempId = tempUserPrivilegeStatements[j].UserPrivilegeId;
                    tempUserPrivileges.Add(db.UserPrivileges.Where(up => up.Id == tempId).First().Value);
                }
                tempStringId = statements[i].KindergardenId;
                model.Add(new StatementListItemViewModel
                {
                    KindergardenName = db.Kindergardens.Where(k => k.Id == tempStringId).First().Name,
                    UserName = tempSiteUser.Surname + " " + tempSiteUser.Name + " " + tempSiteUser.FathersName,
                    UserPrivileges = new List<string>(tempUserPrivileges),
                    Statement = statements[i]
                });
                tempUserPrivileges.Clear();
            }
            return model;
        }

        public void SwitchStatus(int statementId, string status)
        {
            db.Statements.Where(s => s.Id == statementId).First().Status = status;
            db.SaveChanges();
        }

        public void SwitchIsSelected(int statementId, bool isSelected)
        {
            db.Statements.Where(s => s.Id == statementId).First().IsSelected = isSelected;
            db.SaveChanges();
        }

        public void SwitchIsRemoved(int statementId, bool isRemoved)
        {
            db.Statements.Where(s => s.Id == statementId).First().IsRemoved = isRemoved;
            db.SaveChanges();
        }

        public void AddStatement(Statement statement)
        {
            db.Statements.Add(statement);
            db.SaveChanges();
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