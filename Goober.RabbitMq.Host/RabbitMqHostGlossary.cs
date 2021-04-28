namespace Goober.RabbitMq.Host
{
    public static class RabbitMqHostGlossary
    {
        public static string ConnectionStringSectionConfigurationKey { get; set; } = "RabbitMq.ConnectionString";

        private static string _applicationName;

        public static string ApplicationName 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_applicationName) == true)
                {
                    _applicationName = System.Reflection.Assembly.GetEntryAssembly().FullName;
                }

                return _applicationName;
            }
            set 
            {
                _applicationName = value;
            } 
        }
    }
}
