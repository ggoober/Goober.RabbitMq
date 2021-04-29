using Goober.RabbitMq.DAL.Models;
using System.Threading.Tasks;

namespace Goober.RabbitMq.DAL.Repository
{
    public interface IMessageRepository
    {
        Task<MessageModel> GetNotPublishedAsync(string eventMessageTypeFullName, string hash);
        Task InsertAsync(MessageModel newRec);
        Task<MessageModel> GetByIdAsync(long id);
        Task UpdateAsync(MessageModel eventMessage);
    }
}
