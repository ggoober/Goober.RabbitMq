using Goober.Core.Extensions;
using Goober.Core.Services;
using Goober.RabbitMq.Api.Models;
using Goober.RabbitMq.DAL.Models;
using Goober.RabbitMq.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Goober.RabbitMq.WebApi.Controllers.Api
{
    [ApiController]
    public partial class MessageApiController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IDateTimeService _dateTimeService;

        public MessageApiController(IMessageRepository messageRepository,
            IDateTimeService dateTimeService)
        {
            _messageRepository = messageRepository;
            _dateTimeService = dateTimeService;
        }

        [HttpPost]
        [Route("/api/message/register")]
        public async Task<RegisterMessageResponse> RegisterAsync([FromBody]RegisterMessageRequest request) 
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.MessageTypeFullName);
            request.RequiredArgumentNotNull(() => request.JMessage);
            request.RequiredArgumentNotNull(() => request.CheckTransactionUrl);
            request.RequiredArgumentNotNull(() => request.ProducerApplicationName);
            request.RequiredArgumentNotNull(() => request.ProducerCallerMemberName);
            request.RequiredArgumentNotNull(() => request.ProducerHost);

            var hash = Goober.Core.Utils.HashUtils.GetMd5Hash(request.JMessage);

            var notPublishedSameMessage = await _messageRepository.GetNotPublishedAsync(
                eventMessageTypeFullName: request.MessageTypeFullName,
                hash: hash);

            if (notPublishedSameMessage != null)
            {
                return new RegisterMessageResponse 
                {
                    Id = notPublishedSameMessage.Id,
                    Hash = notPublishedSameMessage.Hash,
                    RowVersion = notPublishedSameMessage.RowVersion
                };
            }

            var newRec = new MessageModel { 
                CheckTransactionUrl = request.CheckTransactionUrl,
                MessageTypeFullName = request.MessageTypeFullName,
                Hash = hash,
                JMessage = request.JMessage,
                ProducerApplicationName = request.ProducerApplicationName,
                ProducerCallerMemberName = request.ProducerCallerMemberName,
                ProducerHost = request.ProducerHost
            };

            await _messageRepository.InsertAsync(newRec);

            return new RegisterMessageResponse 
            {
                Id = newRec.Id,
                Hash = newRec.Hash,
                RowVersion = newRec.RowVersion
            };
        }

        [HttpPost]
        [Route("/api/message/set-published")]
        public async Task<SetPublishedMessageResponse> SetPublishedAsync([FromBody]SetPublishedMessageRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumnetNotNullAndNotDefaultValue<SetPublishedMessageRequest, Guid>(() => request.Id);

            var eventMessage = await _messageRepository.GetByIdAsync(id: request.Id);
            eventMessage.RequiredNotNull(nameof(eventMessage));

            if (eventMessage.PublishedDateTime.HasValue == true)
            {
                return new SetPublishedMessageResponse 
                { 
                    PublishedDateTime = eventMessage.PublishedDateTime.Value, 
                    RowVersion = eventMessage.RowVersion 
                };
            }

            eventMessage.PublishedDateTime = _dateTimeService.GetDateTimeNow();
            await _messageRepository.UpdateAsync(eventMessage);

            return new SetPublishedMessageResponse 
            { 
                RowVersion = eventMessage.RowVersion, 
                PublishedDateTime = eventMessage.PublishedDateTime.Value
            };
        }
    }
}
