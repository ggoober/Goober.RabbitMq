using System;

namespace Goober.RabbitMq.Api.Models
{
    public class SetDeletedResponse
    {
        public bool IsSetDeleted { get; set; }

        public string RowVersion { get; set; }

        public DateTime DeletedDateTime { get; set; }
    }
}
