namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedMessageRequest
    {
        public int Id { get; set; }

        public string RowVersion { get; set; }
    }
}
