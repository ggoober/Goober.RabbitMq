namespace Goober.RabbitMq.Api.Models
{
    public class RegisterMessageRequest
    {
        public string MessageTypeFullName { get; set; }

        public string JMessage { get; set; }

        public string CheckTransactionUrl { get; set; }

        public string ProducerCallerMemberName { get; set; }

        public string ProducerApplicationName { get; set; }

        public string ProducerHost { get; set; }
    }
}
