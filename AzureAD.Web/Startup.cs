using Owin;
using System.Web.Services.Description;

namespace AzureAD.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
