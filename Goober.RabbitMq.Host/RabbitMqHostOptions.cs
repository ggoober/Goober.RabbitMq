namespace Goober.RabbitMq.Host
{
    public class RabbitMqHostOptions
    {
        public string Host { get; set; }
        public ushort Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppName { get; set; }

    }
}
