using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancialPlannerBD.Startup))]
namespace FinancialPlannerBD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
