using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IStudyKindergardens.Startup))]
namespace IStudyKindergardens
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
