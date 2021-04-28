namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedEventMessageRequest
    {
        public int Id { get; set; }

        public string RowVersion { get; set; }
    }
}
