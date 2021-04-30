using Goober.RabbitMq.Api.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Producer.Services
{
    public interface IRabbitMqProducerService
    {
        Task PublishAndSetAsPublishedAsync<TMessage>(RegisterMessageResponse registerResult, TMessage message, bool throwException = false, [CallerMemberName] string callerMemberName = null) where TMessage : class;
        
        Task PublishAsync<TMessage>(TMessage message) where TMessage : class;
        
        Task<RegisterMessageResponse> RegisterBeforePublishAsync<TMessage>(TMessage message, [CallerMemberName] string callerMemberName = null) where TMessage : class;
    }
}