using Goober.EntityFramework.Common.Implementation;
using Goober.RabbitMq.DAL.MsSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goober.RabbitMq.DAL.MsSql.DbContext.Implementation
{
    class RabbitMqDBContext : BaseDbContext, IRabbitMqDBContext
    {
        public RabbitMqDBContext(DbContextOptions<RabbitMqDBContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
