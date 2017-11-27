using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BillPay.Startup))]
namespace BillPay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
