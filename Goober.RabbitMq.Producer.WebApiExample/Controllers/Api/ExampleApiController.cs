using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Goober.RabbitMq.Publisher.WebApiExample.Controllers.Api
{
    [ApiController]
    public class ExampleApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ExampleApiController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpPost]
        [Route("/api/example/test-publish")]
        public async Task TestPublishAsync()
        {
            //some transaction scope
            { 
                //some db operation

                //register EventMessage to Produce

                //commit
            }

            //producer.PublishAsync
        }

        public class RegisterEventResult<TEventMessage>
        {
            public int StorageId { get; set; }

            public TEventMessage EventMessage { get; set; }

            public string CheckEventUrl { get; set; }

            public string RowVersion { get; set; }

            public DateTime RowCreatedDate { get; set; }
            
            public string CallerMemberName { get; set; }

            public string ApplicationName { get; set; }

            public string MachineName { get; set; }
            
            public int? TimeToLiveInMinutes { get; set; }
        }

        private RegisterEventResult<TEventMessage> RegisterRabbitMqEventAsync<TEventMessage>(
            TEventMessage eventMessage,
            string checkEventUrl = null,
            int? timeToLiveInMinutes = null,
            [CallerMemberName]string callerMemberName = null)
        {
            string correctedCheckEventUrl = checkEventUrl ?? GetCheckEventUrlFromConfiguration<TEventMessage>();

            var ret = new RegisterEventResult<TEventMessage>
            {
                EventMessage = eventMessage,
                CheckEventUrl = correctedCheckEventUrl,
                CallerMemberName = callerMemberName,
                TimeToLiveInMinutes = timeToLiveInMinutes
            };

            //call api to regiserEvent

            return ret;
        }

        private string GetCheckEventUrlFromConfiguration<TEventMessage>()
        {
            var fullName = typeof(TEventMessage).FullName;
            var rabbitMqPublishEventsChecks = _configuration.GetSection("RabbitMq.CheckEventUrls");
            var url = rabbitMqPublishEventsChecks.GetValue<string>(fullName);

            return url;
        }
    }
}
