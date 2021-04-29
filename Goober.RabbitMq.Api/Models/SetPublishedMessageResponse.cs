using System;

namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedMessageResponse
    {
        public string RowVersion { get; set; }

        public DateTime PublishedDateTime { get; set; }
    }
}
