using System;

namespace Goober.RabbitMq.Api.Models
{
    public class SetPublishedMessageResponse
    {
        public bool IsSetPublished { get; set; }

        public string RowVersion { get; set; }

        public DateTime PublishedDateTime { get; set; }
    }
}
