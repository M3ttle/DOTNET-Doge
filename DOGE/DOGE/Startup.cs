using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DOGE.Startup))]
namespace DOGE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
