using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DOGEOnlineGeneralEditor.Startup))]
namespace DOGEOnlineGeneralEditor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);

        }
    }
}
