using System;

namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedMessageRequest
    {
        public Guid Id { get; set; }

        public string RowVersion { get; set; }
    }
}
