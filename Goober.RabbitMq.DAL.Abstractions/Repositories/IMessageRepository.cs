using Goober.RabbitMq.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goober.RabbitMq.DAL.Repositories
{
    public interface IMessageRepository
    {
        Task<MessageModel> GetNotPublishedAsync(string eventMessageTypeFullName, string hash);
        
        Task InsertAsync(MessageModel newRec);
        
        Task<MessageModel> GetByIdAsync(Guid id);

        Task UpdateAsync(MessageModel eventMessage);

        Task<List<MessageModel>> GetAllForTestAsync();

        Task SetDeletedAsync(Guid id, DateTime dateNow);

        Task<List<MessageModel>> GetNotPublishedAsync(string applicationName,
            int topRowsCount = 100,
            string host = null,
            string messageTypeFullName = null);
    }
}
