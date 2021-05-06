using System;
using System.Collections.Generic;

namespace Goober.RabbitMq.Api.Models
{
    public class GetConcurrentWithoutHostNotPublishedIdsResponse
    {
        public List<Guid> Ids { get; set; }
    }
}
