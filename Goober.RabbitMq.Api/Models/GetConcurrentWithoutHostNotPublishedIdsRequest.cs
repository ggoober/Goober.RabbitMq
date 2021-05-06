namespace Goober.RabbitMq.Api.Models
{
    public class GetConcurrentWithoutHostNotPublishedIdsRequest
    {
        public string ApplicationName { get; set; }

        public int RowsCount { get; set; }

        public string MessageTypeFullName { get; set; }

        public int? CreatedDelayInMilliseconds { get; set; }

        public int? ConcurrentSelectDelayInMilliseconds { get; set; }
    }
}
