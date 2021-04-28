namespace Goober.RabbitMq.Api.Models
{
    public class RegisterPublishEventMessageRequest
    {
        public string EventMessageTypeFullName { get; set; }

        public string JEventMessage { get; set; }

        public string CheckTransactionUrl { get; set; }

        public string ProducerCallerMemberName { get; set; }

        public string ProducerApplicationName { get; set; }

        public string ProducerHost { get; set; }
    }
}
