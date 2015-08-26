using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CEDemo.Startup))]
namespace CEDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
