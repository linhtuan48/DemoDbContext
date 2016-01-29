using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DemoDbContextRepository.Startup))]
namespace DemoDbContextRepository
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
