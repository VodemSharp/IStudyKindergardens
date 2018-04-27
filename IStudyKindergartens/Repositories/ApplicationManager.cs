using IStudyKindergartens.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Repositories
{
    public interface IApplicationManager
    {
        string GetApplicationUserNameById(string id);
        bool IsEmailExist(string email);
        string GetAdminId();
    }

    public class ApplicationManager : IApplicationManager, IDisposable
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string GetAdminId()
        {
            List<ApplicationUser> applicationUsers = db.Users.ToList();
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            for (int i = 0; i < applicationUsers.Count; i++)
            {
                if(userManager.GetRoles(applicationUsers[i].Id).Any(r => r == "Admin"))
                {
                    return applicationUsers[i].Id;
                }
            }
            return null;
        }

        public string GetApplicationUserNameById(string id)
        {
            if (db.SiteUsers.Any(su => su.Id == id))
            {
                return db.SiteUsers.Where(su => su.Id == id).First().FullName;
            }
            else
            {
                return db.Kindergartens.Where(k => k.Id == id).First().Name;
            }
        }

        public bool IsEmailExist(string email)
        {
            if (db.Users.Any(u => u.Email == email))
            {
                return true;
            }
            return false;
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