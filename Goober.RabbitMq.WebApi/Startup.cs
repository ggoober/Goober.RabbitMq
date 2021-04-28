using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.RabbitMq.WebApi
{
    public class Startup : Goober.Web.BaseApiStartup
    {
        public Startup()
            : base(swaggerSettings: null,
                  configSettings: null,
                  memoryCacheSizeLimitInMB: null)
        {
        }

        protected override void ConfigurePipelineAfterExceptionsHandling(IApplicationBuilder app)
        {
        }

        protected override void ConfigurePipelineAfterMvc(IApplicationBuilder app)
        {
        }

        protected override void ConfigureServiceCollections(IServiceCollection services)
        {
        }
    }
}
