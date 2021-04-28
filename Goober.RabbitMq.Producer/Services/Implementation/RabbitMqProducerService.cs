using EasyNetQ;
using Goober.RabbitMq.Host;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Producer.Services.Implementation
{
    class RabbitMqProducerService
    {
        private readonly IConfiguration _configuration;

        public RabbitMqProducerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Передаёт сообщение брокеру сообщений, неблокирующий вызов.
        /// </summary>
        /// <typeparam name="TEventMessageType">Тип сообщения</typeparam>
        /// <param name="eventMessage">Сообщение</param>        
        public async Task PublishAsync<TEventMessageType>(TEventMessageType eventMessage) where TEventMessageType : class
        {
            using (var bus = CreateConnection())
            {
                await bus.PubSub.PublishAsync(eventMessage);
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

                Hosts = connectionString.Hosts.Select(x=> new HostConfiguration { Host = x.Host, Port = x.Port }).ToList()
            };

            var ret = RabbitHutch.CreateBus(connectionConfiguration, serviceRegister => { });

            return ret;
        }
    }
}
