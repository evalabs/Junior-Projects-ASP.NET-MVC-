using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JuniorProjectsTest.Startup))]
namespace JuniorProjectsTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
