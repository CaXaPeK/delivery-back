using Delivery_Service.Controllers;

namespace Delivery_Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<AutoDeleter>();
        }
    }
}
