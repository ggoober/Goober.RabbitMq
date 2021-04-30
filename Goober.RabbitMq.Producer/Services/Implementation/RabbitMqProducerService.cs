using EasyNetQ;
using Goober.Core.Extensions;
using Goober.RabbitMq.Api.Models;
using Goober.RabbitMq.Api.Services;
using Goober.RabbitMq.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Producer.Services.Implementation
{
    class RabbitMqProducerService : IRabbitMqProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageHttpService _messageHttpService;
        private readonly ILogger<RabbitMqProducerService> _logger;
        private readonly string _applicationName;

        public RabbitMqProducerService(IConfiguration configuration,
            IMessageHttpService messageHttpService,
            ILogger<RabbitMqProducerService> logger)
        {
            _configuration = configuration;
            _messageHttpService = messageHttpService;
            _logger = logger;
            _applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        /// <summary>
        /// Регистрирует сообщение перед отправкой, вызывать до завершения транзакции основной логики применения изменений.
        /// Используется для предотвращения потери сообщения, если после завершения транзакции не удалось доставить сообщение в Rabbit
        /// </summary>
        /// <typeparam name="TMessage">тип сообщения</typeparam>
        /// <param name="message">сообщение</param>
        /// <returns></returns>
        public async Task<RegisterMessageResponse> RegisterBeforePublishAsync<TMessage>(
            TMessage message,
            [CallerMemberName] string callerMemberName = null) where TMessage : class
        {
            var messageTypeFullName = message.GetType().FullName;

            var section = _configuration.GetSection(RabbitMqProducerGlossary.RabbitMqProducerConfigSectionKey);
            var checkTransactionUrl = section[messageTypeFullName];
            if (string.IsNullOrEmpty(checkTransactionUrl) == true)
            {
                throw new InvalidOperationException($"Can't find checkTransactionUrl for messageType = {messageTypeFullName} in section = {RabbitMqProducerGlossary.RabbitMqProducerConfigSectionKey}");
            }

            var request = new RegisterMessageRequest
            {
                CheckTransactionUrl = checkTransactionUrl,
                JMessage = message.Serialize(),
                MessageTypeFullName = messageTypeFullName,
                ProducerApplicationName = _applicationName,
                ProducerCallerMemberName = callerMemberName,
                ProducerHost = System.Environment.MachineName
            };

            var res = await _messageHttpService.RegisterAsync(request: request,
                callerMemberName: callerMemberName);

            return res;
        }

        /// <summary>
        /// Отправляет сообщение в очередь и помечает его как отправленное. Используется после вызова метода RegisterBeforePublishAsync.
        /// </summary>
        /// <param name="registerResult">Результат предварительно зарегистрированного сообщения</param>
        /// <returns></returns>
        public async Task PublishAndSetAsPublishedAsync<TMessage>(
            RegisterMessageResponse registerResult,
            TMessage message,
            bool throwException = false,
            [CallerMemberName] string callerMemberName = null) where TMessage : class
        {
            var stepExceptionMessage = "Unable to publish message in RabbitMq";

            try
            {
                using (var bus = CreateConnection())
                {
                    await bus.PubSub.PublishAsync(message);
                }

                stepExceptionMessage = "Unable to set published message after successfully published in RabbitMq";
                
                await _messageHttpService.SetPublishedAsync(
                request: new SetPublishedMessageRequest
                {
                    Id = registerResult.Id,
                    RowVersion = registerResult.RowVersion
                },
                callerMemberName: callerMemberName);
            }
            catch (Exception exc)
            {
                if (throwException == true)
                    throw;

                _logger.LogError(message: stepExceptionMessage, exception: exc);
            }
        }

        /// <summary>
        /// Передаёт сообщение брокеру сообщений. Если при передаче произойдёт ошибка, то сообщение будет потеряно.
        /// </summary>
        /// <typeparam name="TMessage">Тип сообщения</typeparam>
        /// <param name="message">Сообщение</param>        
        public async Task PublishAsync<TMessage>(TMessage message) where TMessage : class
        {
            using (var bus = CreateConnection())
            {
                await bus.PubSub.PublishAsync(message);
            }
        }

        private IBus CreateConnection()
        {
            var connectionString = _configuration.GetSection(RabbitMqHostGlossary.ConnectionStringSectionConfigurationKey).Get<RabbitMqHostConnectionString>();
            if (connectionString == null)
            {
                throw new InvalidOperationException($"Can't find RabbitMqHostConnectionString from configuration by key = {RabbitMqHostGlossary.ConnectionStringSectionConfigurationKey}");
            }

            if (connectionString.Hosts.Any() == false)
            {
                throw new InvalidOperationException($"ConnectionString.Hosts is empty");
            }

            var connectionConfiguration = new ConnectionConfiguration()
            {
                PublisherConfirms = true,
                PersistentMessages = true,

                Password = connectionString.Password,
                UserName = connectionString.UserName,
                VirtualHost = connectionString.VirtualHost,

                Name = connectionString.AppName ?? RabbitMqHostGlossary.ApplicationName,

                Hosts = connectionString.Hosts.Select(x => new HostConfiguration { Host = x.Host, Port = x.Port }).ToList()
            };

            var ret = RabbitHutch.CreateBus(connectionConfiguration, serviceRegister => { });

            return ret;
        }
    }
}
