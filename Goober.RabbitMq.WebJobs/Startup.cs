using Goober.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.RabbitMq.WebJobs
{
    public class Startup : Goober.Web.BaseApiStartup
    {
        public Startup()
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
            services.RegisterAssemblyClasses<Startup>();
        }
    }
}
