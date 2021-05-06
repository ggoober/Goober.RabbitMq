namespace Goober.RabbitMq.Producer
{
    public static class RabbitMqProducerGlossary
    {
        public static string RabbitMqProducerConfigSectionKey { get; set; } = "RabbitMq.Producer.CheckTransactionUrls";

        public static int DefaultRetryCont { get; set; } = 3;

        public static int DefaultRetryDelayInMilliseconds { get; set; } = 3000;
    }
}
