using Goober.EntityFramework.SqlServer;
using Goober.RabbitMq.DAL.Repositories;
using Goober.RabbitMq.DAL.MsSql.DbContext;
using Goober.RabbitMq.DAL.MsSql.DbContext.Implementation;
using Goober.RabbitMq.DAL.MsSql.Repositories.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Goober.RabbitMq.WebApi.Tests")]
namespace Goober.RabbitMq.DAL.MsSql
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRabbitMqMsSqlDbContext(this IServiceCollection services, 
            IConfiguration configuration,
            string connectionStringConfigKey = "RabbitMqDB")
        {
            services.RegisterMsSqlDbContext<IRabbitMqDBContext, RabbitMqDBContext>(() => configuration.GetConnectionString(connectionStringConfigKey));
        }

        public static void RegisterRabbitMqMsSqlRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMessageRepository, MessageRepository>();
        }

    }
}
