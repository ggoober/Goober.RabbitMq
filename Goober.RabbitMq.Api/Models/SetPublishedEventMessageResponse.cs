using System;

namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedEventMessageResponse
    {
        public string RowVersion { get; set; }

        public DateTime PublishedDateTime { get; set; }
    }
}
