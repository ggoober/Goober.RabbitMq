using Goober.RabbitMq.DAL.Abstractions.Models;
using System.Threading.Tasks;

namespace Goober.RabbitMq.DAL.Abstractions.Services
{
    public interface IPublishEventMessageRepository
    {
        Task<PublishEventMessageModel> GetNotPublishedAsync(string eventMessageTypeFullName, string hash);
        Task InsertAsync(PublishEventMessageModel newRec);
        PublishEventMessageModel GetByIdAsync(int id);
    }
}
