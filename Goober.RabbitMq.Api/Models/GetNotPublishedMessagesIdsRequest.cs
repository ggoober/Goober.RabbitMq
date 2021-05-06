namespace Goober.RabbitMq.Api.Models
{
    public class GetNotPublishedMessagesIdsRequest
    {
        public string ApplicationName { get; set; }

        public string Host { get; set; }

        public int RowsCount { get; set; }

        public string MessageTypeFullName { get; set; }

        public int? CreatedDelayInMilliseconds { get; set; }
    }
}
