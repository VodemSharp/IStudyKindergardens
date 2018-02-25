namespace IStudyKindergardens.Migrations
{
    using IStudyKindergardens.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IStudyKindergardens.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IStudyKindergardens.Models.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole("Moderator"));
            }
            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole("Administrator"));
            }
            if (!context.Roles.Any(r => r.Name == "User"))
            {
                roleManager.Create(new IdentityRole("User"));
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            if (!context.Users.Any(u => u.Email == "vadumfedor@gmail.com"))
            {
                IStudyKindergardens.Models.ApplicationUser user = new Models.ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "vadumfedor@gmail.com", UserName = "vadumfedor@gmail.com", EmailConfirmed = true, PasswordHash = "AFKhNnmKSBqL7sJwKwbQwX+kkW59xtQDv3kSvLIVrH/0Cjz14knGFSp4p6PPUkYn6g==", SecurityStamp = "905f0abc-a222-4365-a716-7deae252c113", PhoneNumber = "+38 (050) 750-0406", PhoneNumberConfirmed = true, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0 };
                userManager.Create(user, "password");
                userManager.AddToRole(user.Id, "Admin");
                SiteUser siteUser = new SiteUser { Name = "Вадим", Surname = "Федорович", FathersName = "Романович", DateOfBirth = "04/10/1999", Id = user.Id };
                context.SiteUsers.Add(siteUser);
                context.SaveChanges();
            }
        }
    }
}
