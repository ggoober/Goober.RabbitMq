using Goober.Web;

namespace Goober.RabbitMq.Publisher.WebApiExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProgramUtils.RunWebhost<Startup>(args);
        }
    }
}
