using Autofac;
using Autofac.Integration.Mvc;
using IStudyKindergartens.Models;
using IStudyKindergartens.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(IStudyKindergartens.Startup))]
namespace IStudyKindergartens
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // получаем экземпляр контейнера
            var builder = new ContainerBuilder();

            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole, string>>();
            builder.RegisterType<RoleManager<IdentityRole>>().AsSelf().InstancePerRequest();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).InstancePerRequest();

            // регистрируем контроллер в текущей сборке
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // регистрируем споставление типов
            builder.RegisterType<SiteUserManager>().As<ISiteUserManager>();
            builder.RegisterType<KindergartenManager>().As<IKindergartenManager>();
            builder.RegisterType<RatingManager>().As<IRatingManager>();
            builder.RegisterType<StatementManager>().As<IStatementManager>();
            builder.RegisterType<MessageManager>().As<IMessageManager>();
            builder.RegisterType<ApplicationManager>().As<IApplicationManager>();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();

            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            ConfigureAuth(app);
        }
    }
}
