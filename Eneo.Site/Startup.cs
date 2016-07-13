using Eneo.Site.App_Start;
using Owin;
using System.Web.Http;

namespace Eneo.Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
      
        }
    }
}