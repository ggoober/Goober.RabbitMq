using Goober.Core.Extensions;
using Goober.RabbitMq.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Goober.RabbitMq.WebJobs.Services.Implementation
{
    public class CheckAndPublishNotPublishedMessagesJobService : ICheckAndPublishNotPublishedMessagesJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageRepository _messageRepository;

        public CheckAndPublishNotPublishedMessagesJobService(
            IConfiguration configuration,
            IMessageRepository messageRepository)
        {
            this._configuration = configuration;
            this._messageRepository = messageRepository;
        }

        public async Task<List<Guid>> GetItemsAsync()
        {
            var takeCount = _configuration["CheckAndPublishNotPublishedMessages.TakeCount"].ToInt() ?? 100;

            var ret = await _messageRepository.GetNotPublishedIdsAsync(topRowsCount: takeCount);

            return ret;
        }

        public async Task ProcessItemAsync(Guid item, CancellationToken stoppinngToken)
        {
            var message = await _messageRepository.GetByIdAsync(id: item);
            message.RequiredNotNull(nameof(message), context: new { Id = item });

            if (message.PublishedDateTime.HasValue == true
                || message.DateOfDelete.HasValue == true)
            {
                return;
            }

            

        }
    }
}
