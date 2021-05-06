namespace Goober.RabbitMq.WebApi.Glossary
{
    public static class MessageGlossary
    {
        public static int DefaultConcurrentSelectLockDelayInMilliseconds { get; set; } = 60000 * 3;

        public static int DefaultCreatedDelayInMilliseconds { get; set; } = 60000 * 1;
    }
}
