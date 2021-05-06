using Goober.RabbitMq.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goober.RabbitMq.DAL.Repositories
{
    public interface IMessageRepository
    {
        Task InsertAsync(MessageModel newRec);
        
        Task<MessageModel> GetByIdAsync(Guid id);

        Task<MessageModel> GetNotPublishedAsync(string messageTypeFullName, string hash);

        Task UpdateAsync(MessageModel eventMessage);

        Task<List<MessageModel>> GetAllForTestAsync();

        Task SetDeletedAsync(Guid id, DateTime dateNow);

        Task<List<Guid>> GetNotPublishedIdsAsync(
            DateTime createdDelayEndDateTime,
            int topRowsCount = 100,
            string applicationName = null,
            string host = null,
            string messageTypeFullName = null,
            DateTime? concurrentSelectLockEndDateTime = null);
    }
}
