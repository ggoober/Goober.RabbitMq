namespace Goober.RabbitMq.Api.Models
{
    public class RegisterMessageResponse
    {
        public long Id { get; set; }

        public string Hash { get; set; }

        public string RowVersion { get; set; }
    }
}
