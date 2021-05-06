using Goober.WebJobs.Abstractions;
using System;

namespace Goober.RabbitMq.WebJobs.Services
{
    interface ICheckAndPublishNotPublishedMessagesJobService: IListJobService<Guid>
    {
    }
}
