using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Quotes2.Startup))]
namespace Quotes2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
