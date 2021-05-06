using System;
using System.Collections.Generic;

namespace Goober.RabbitMq.Api.Models
{
    public class GetNotPublishedMessagesIdsResponse
    {
        public List<Guid> Ids { get; set; } = new List<Guid>();
    }
}
