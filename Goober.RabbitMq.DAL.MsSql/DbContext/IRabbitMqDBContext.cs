using Goober.RabbitMq.DAL.MsSql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goober.RabbitMq.DAL.MsSql.DbContext
{
    internal interface IRabbitMqDBContext
    {
        DbSet<Message> Messages { get; set; }
    }
}