using Goober.RabbitMq.Api.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Api.Services
{
    public interface IMessageHttpService
    {
        Task<RegisterMessageResponse> RegisterAsync(RegisterMessageRequest request, [CallerMemberName] string callerMemberName = null);
        
        Task<SetPublishedMessageResponse> SetPublishedAsync(SetPublishedMessageRequest request, [CallerMemberName] string callerMemberName = null);
        
        Task<GetConcurrentWithoutHostNotPublishedIdsResponse> GetConcurrentWithoutHostNotPublishedIdsAsync(GetConcurrentWithoutHostNotPublishedIdsRequest request,
            [CallerMemberName] string callerMemberName = null);

        Task<GetNotPublishedMessagesIdsResponse> GetNotPublishedMessagesIdsAsync(GetNotPublishedMessagesIdsRequest request,
            [CallerMemberName] string callerMemberName = null);
    }
}