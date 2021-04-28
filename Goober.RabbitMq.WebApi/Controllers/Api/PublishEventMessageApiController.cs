using Goober.Core.Extensions;
using Goober.RabbitMq.Api.Models;
using Goober.RabbitMq.DAL.Abstractions.Models;
using Goober.RabbitMq.DAL.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goober.RabbitMq.WebApi.Controllers.Api
{
    [ApiController]
    public partial class PublishEventMessageApiController : ControllerBase
    {
        private readonly IPublishEventMessageRepository _publishEventMessageRepository;

        public PublishEventMessageApiController(IPublishEventMessageRepository publishEventMessageRepository)
        {
            this._publishEventMessageRepository = publishEventMessageRepository;
        }

        [Route("api/publish-event-message/register")]
        public async Task<RegisterPublishEventMessageResponse> RegisterAsync([FromBody]RegisterPublishEventMessageRequest request) 
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.EventMessageTypeFullName);
            request.RequiredArgumentNotNull(() => request.JEventMessage);
            request.RequiredArgumentNotNull(() => request.CheckTransactionUrl);
            request.RequiredArgumentNotNull(() => request.ProducerApplicationName);
            request.RequiredArgumentNotNull(() => request.ProducerCallerMemberName);
            request.RequiredArgumentNotNull(() => request.ProducerHost);

            var hash = Goober.Core.Utils.HashUtils.GetMd5Hash(request.JEventMessage);

            var notPublishedSameEventMessage = await _publishEventMessageRepository.GetNotPublishedAsync(
                eventMessageTypeFullName: request.EventMessageTypeFullName,
                hash: hash);

            if (notPublishedSameEventMessage != null)
            {
                return new RegisterPublishEventMessageResponse 
                {
                    Id = notPublishedSameEventMessage.Id,
                    Hash = notPublishedSameEventMessage.Hash,
                    RowVersion = notPublishedSameEventMessage.RowVersion
                };
            }

            var newRec = new PublishEventMessageModel { 
                CheckTransactionUrl = request.CheckTransactionUrl,
                EventMessageTypeFullName = request.EventMessageTypeFullName,
                Hash = hash,
                JEventMessage = request.JEventMessage,
                ProducerApplicationName = request.ProducerApplicationName,
                ProducerCallerMemberName = request.ProducerCallerMemberName,
                ProducerHost = request.ProducerHost
            };

            await _publishEventMessageRepository.InsertAsync(newRec);

            return new RegisterPublishEventMessageResponse 
            {
                Id = newRec.Id,
                Hash = newRec.Hash,
                RowVersion = newRec.RowVersion
            };
        }

        [Route("api/publish-event-message/set-published")]
        public async Task SetPublishedAsync(SetPublishedEventMessageRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumnetNotNullAndNotDefaultValue<SetPublishedEventMessageRequest, int>(() => request.Id);
            request.RequiredArgumentNotNull(propertyLambda: () => request.RowVersion);

            var eventMessage = _publishEventMessageRepository.GetByIdAsync(id: request.Id);
            eventMessage.RequiredNotNull(nameof(eventMessage));


        }
    }
}
