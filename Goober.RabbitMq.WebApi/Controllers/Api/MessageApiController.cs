using Goober.Core.Extensions;
using Goober.Core.Services;
using Goober.RabbitMq.Api.Models;
using Goober.RabbitMq.DAL.Models;
using Goober.RabbitMq.DAL.Repositories;
using Goober.RabbitMq.WebApi.Glossary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goober.RabbitMq.WebApi.Controllers.Api
{
    [ApiController]
    public partial class MessageApiController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IConfiguration _configuration;

        public MessageApiController(IMessageRepository messageRepository,
            IDateTimeService dateTimeService,
            IConfiguration configuration)
        {
            _messageRepository = messageRepository;
            _dateTimeService = dateTimeService;
            _configuration = configuration;
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
                messageTypeFullName: request.MessageTypeFullName,
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

            var message = await _messageRepository.GetByIdAsync(id: request.Id);
            message.RequiredNotNull(nameof(message));

            if (message.PublishedDateTime.HasValue == true)
            {
                return new SetPublishedMessageResponse 
                { 
                    IsSetPublished = false,
                    PublishedDateTime = message.PublishedDateTime.Value, 
                    RowVersion = message.RowVersion 
                };
            }

            if (message.DateOfDelete.HasValue == true)
            {
                throw new InvalidOperationException($"message.DateOfDelete != null, message: {message.Serialize()}");
            }

            message.PublishedDateTime = _dateTimeService.GetDateTimeNow();
            await _messageRepository.UpdateAsync(message);

            return new SetPublishedMessageResponse
            {
                IsSetPublished = true,
                RowVersion = message.RowVersion, 
                PublishedDateTime = message.PublishedDateTime.Value
            };
        }

        [HttpPost]
        [Route("/api/message/set-deleted")]
        public async Task<SetDeletedResponse> SetDeletedAsync([FromBody]SetDeletedRequest request)
        {
            request.RequiredArgumentNotNull(nameof(request));
            request.RequiredArgumnetNotNullAndNotDefaultValue<SetDeletedRequest, Guid>(() => request.Id);

            var message = await _messageRepository.GetByIdAsync(request.Id);
            message.RequiredNotNull(nameof(message));

            if (message.DateOfDelete.HasValue == true)
            {
                return new SetDeletedResponse 
                { 
                    IsSetDeleted = false,
                    RowVersion = message.RowVersion, 
                    DeletedDateTime = message.DateOfDelete.Value 
                };
            }

            message.DateOfDelete = _dateTimeService.GetDateTimeNow();
            await _messageRepository.UpdateAsync(message);

            return new SetDeletedResponse
            {
                IsSetDeleted = true,
                DeletedDateTime = message.DateOfDelete.Value,
                RowVersion = message.RowVersion
            };
        }

        [HttpPost]
        [Route("/api/message/get-concurrent-without-host-not-published-ids")]
        public async Task<GetConcurrentWithoutHostNotPublishedIdsResponse> GetConcurrentWithoutHostNotPublishedIdsAsync([FromBody] GetConcurrentWithoutHostNotPublishedIdsRequest request)
        {
            request.RequiredNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.ApplicationName);
            request.RequiredArgumnetNotNullAndNotDefaultValue<GetConcurrentWithoutHostNotPublishedIdsRequest, int>(() => request.RowsCount);

            var concurrentSelectDelayInMilliseconds = request.ConcurrentSelectDelayInMilliseconds ?? MessageGlossary.DefaultConcurrentSelectLockDelayInMilliseconds;
            var concurrentSelectLockEndDateTime = _dateTimeService.GetDateTimeNow().AddMilliseconds(-concurrentSelectDelayInMilliseconds);

            var createdDelayInMilliseconds = request.CreatedDelayInMilliseconds ?? MessageGlossary.DefaultCreatedDelayInMilliseconds;
            var createdDelayEndDateTime = _dateTimeService.GetDateTimeNow().AddMilliseconds(-createdDelayInMilliseconds);

            var rowsWithConcurrentRowsIds = await _messageRepository.GetNotPublishedIdsAsync(
                createdDelayEndDateTime: createdDelayEndDateTime,
                topRowsCount: request.RowsCount,
                applicationName: request.ApplicationName,
                host: null,
                messageTypeFullName: request.MessageTypeFullName,
                concurrentSelectLockEndDateTime: concurrentSelectLockEndDateTime);

            var res = new List<Guid>();

            foreach (var iConcurrentRowId in rowsWithConcurrentRowsIds)
            {
                var dateNow = _dateTimeService.GetDateTimeNow();

                var concurrentMessage = await _messageRepository.GetByIdAsync(iConcurrentRowId);

                if (concurrentMessage.ConcurrentSelectLockDateTime.HasValue == true)
                {
                    var concurrentSelectLockDateTimePlusDelay = concurrentMessage.ConcurrentSelectLockDateTime.Value.AddMilliseconds(concurrentSelectDelayInMilliseconds);

                    if (concurrentSelectLockDateTimePlusDelay > dateNow)
                    {
                        continue;
                    }
                }

                concurrentMessage.ConcurrentSelectLockDateTime = dateNow;
                try
                {
                    await _messageRepository.UpdateAsync(concurrentMessage);
                    res.Add(iConcurrentRowId);
                }
                catch (Exception exc)
                {
                    continue;
                }
            }

            return new GetConcurrentWithoutHostNotPublishedIdsResponse { Ids = res };
        }

        [HttpPost]
        [Route("/api/message/get-not-published-ids")]
        public async Task<GetNotPublishedMessagesIdsResponse> GetNotPublishedMessagesIds([FromBody] GetNotPublishedMessagesIdsRequest request)
        {
            request.RequiredNotNull(nameof(request));
            request.RequiredArgumentNotNull(() => request.ApplicationName);
            request.RequiredArgumentNotNull(() => request.Host);
            request.RequiredArgumnetNotNullAndNotDefaultValue<GetNotPublishedMessagesIdsRequest, int>(() => request.RowsCount);

            var createdDelayInMilliseconds = request.CreatedDelayInMilliseconds ?? MessageGlossary.DefaultCreatedDelayInMilliseconds;
            var createdDelayEndDateTime = _dateTimeService.GetDateTimeNow().AddMilliseconds(-createdDelayInMilliseconds);
            
            var res = await _messageRepository.GetNotPublishedIdsAsync(
                createdDelayEndDateTime: createdDelayEndDateTime,
                topRowsCount: request.RowsCount,
                applicationName: request.ApplicationName,
                host: request.Host,
                messageTypeFullName: request.MessageTypeFullName,
                concurrentSelectLockEndDateTime: null);

            return new GetNotPublishedMessagesIdsResponse { Ids = res };
        }
    }
}
