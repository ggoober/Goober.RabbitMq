using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Goober.RabbitMq.DAL.MsSql;
using Goober.Core.Extensions;

namespace Goober.RabbitMq.WebApi
{
    public class Startup : Goober.Web.BaseApiStartup
    {
        public Startup()
            : base(swaggerSettings: null,
                  configSettings: new Web.Models.BaseStartupConfigSettings 
                  { 
                      ConfigApiEnvironmentAndHostMappings = null 
                  },
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
            services.RegisterRabbitMqMsSqlDbContext(Configuration);
            services.RegisterRabbitMqMsSqlRepositories();
        }
    }
}
