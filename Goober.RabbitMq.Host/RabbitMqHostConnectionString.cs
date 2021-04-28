using System.Collections.Generic;

namespace Goober.RabbitMq.Host
{
    public class RabbitMqHostConnectionString
    {
        public List<RabbitMqHostParameters> Hosts { get; set; }

        public string VirtualHost { get; set; } = "/";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppName { get; set; }
    }
}
