using Goober.RabbitMq.WebJobs.Services;
using Goober.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace Goober.RabbitMq.WebJobs.Jobs
{
    class CheckAndPublishNotPublishedMessagesJob : ListJob<Guid, ICheckAndPublishNotPublishedMessagesJobService>
    {
        public CheckAndPublishNotPublishedMessagesJob(
            ILogger<CheckAndPublishNotPublishedMessagesJob> logger, 
            IServiceProvider serviceProvider) 
            : base(logger, serviceProvider)
        {
        }
    }
}
