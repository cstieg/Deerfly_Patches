using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Deerfly_Patches.Startup))]
namespace Deerfly_Patches
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
