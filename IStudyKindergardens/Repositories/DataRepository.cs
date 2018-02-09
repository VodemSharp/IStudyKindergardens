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
        void AddSiteUser(SiteUser siteUser, bool IsAdministration);
        void AddTempPicture(string name);

        IEnumerable<SiteUser> GetSiteUsers();
    }

    public class DataRepository : IDisposable, IDataRepository
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private void CreateRole(string name)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (roleManager.FindByNameAsync(name) != null)
            {
                var role = new IdentityRole { Name = name };
                roleManager.Create(role);
            }
        }

        public void AddSiteUser(SiteUser siteUser, bool IsAdministration)
        {
            db.SiteUsers.Add(siteUser);
            if (IsAdministration)
            {
                string role = "Administration";
                CreateRole(role);
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                userManager.AddToRole(siteUser.Id, role);
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