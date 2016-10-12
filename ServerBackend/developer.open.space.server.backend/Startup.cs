using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(developer.open.space.server.backend.Startup))]

namespace developer.open.space.server.backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}