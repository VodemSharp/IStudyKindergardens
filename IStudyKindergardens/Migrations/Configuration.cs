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

            List<string> privileges = new List<string> {
                "Діти сироти, діти позбавлені батьківського піклування, які перебувають під опікою і виховуються в сім'ях",
                "Діти-інваліди (які не мають протипоказань щодо відвідування дошкільних навчальних закладів загального розвитку)",
                "Діти, в яких обоє батьків інваліди І або ІІ групи, або один з батьків інвалід І чи ІІ групи (неповна сім'я)",
                "Діти, які потерпіли від наслідків аварії на ЧАЕС",
                "Діти працівників органів внутрішніх справ, які загинули під час виконання службових обов'язків",
                "Багатодітна сім'я (троє і більше дітей) з них двоє і більше дітей дошкільного віку",
                "Діти, батьки яких мають статус відповідно до пункту 19 статті 6 Закону України (про статус ветеранів війни, гарантії їх соціального захисту)"
            };

            string tempPrivilege;
            for (int i = 0; i < privileges.Count; i++)
            {
                tempPrivilege = privileges[i];
                if (!context.Privileges.Any(p => p.Value == tempPrivilege))
                {
                    context.Privileges.Add(new Privilege { Value = tempPrivilege });
                }
                if (!context.UserPrivileges.Any(p => p.Value == tempPrivilege))
                {
                    context.UserPrivileges.Add(new UserPrivilege { Value = tempPrivilege });
                }
            }

            List<string> groups = new List<string> {
                "За роком народження",
                "На рік молодша"
            };

            string tempGroup;
            for (int i = 0; i < groups.Count; i++)
            {
                tempGroup = groups[i];
                if (!context.Groups.Any(p => p.Value == tempGroup))
                {
                    context.Groups.Add(new Group { Value = tempGroup });
                }
            }
        }
    }
}
