using Goober.Http;
using Goober.Http.Services;
using Goober.RabbitMq.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Api.Services.Implementation
{
    class MessageHttpService : BaseHttpService, IMessageHttpService
    {
        public MessageHttpService(IConfiguration configuration, 
            IHttpJsonHelperService httpJsonHelperService, 
            IHttpContextAccessor httpContextAccessor, 
            IHostEnvironment hostEnvironment) 
            : base(configuration, httpJsonHelperService, httpContextAccessor, hostEnvironment)
        {
        }

        protected override string ApiSchemeAndHostConfigKey { get; set; } = "RabbitMq.Api.SchemeAndHost";

        public async Task<RegisterMessageResponse> RegisterAsync(RegisterMessageRequest request,
                [CallerMemberName] string callerMemberName = null)
                => await ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                    path: "/api/message/register",
                    request: request,
                    callerMemberName: callerMemberName);

        public async Task<SetPublishedMessageResponse> SetPublishedAsync(SetPublishedMessageRequest request,
            [CallerMemberName] string callerMemberName = null)
            => await ExecutePostAsync<SetPublishedMessageResponse, SetPublishedMessageRequest>(
                path: "/api/message/set-published",
                request: request,
                callerMemberName: callerMemberName
            );
    }
}
