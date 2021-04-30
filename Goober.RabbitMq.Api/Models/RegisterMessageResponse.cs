using System;

namespace Goober.RabbitMq.Api.Models
{
    public class RegisterMessageResponse
    {
        public Guid Id { get; set; }

        public string Hash { get; set; }

        public string RowVersion { get; set; }
    }
}
