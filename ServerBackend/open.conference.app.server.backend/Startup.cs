using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(open.conference.app.server.backend.Startup))]

namespace open.conference.app.server.backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}