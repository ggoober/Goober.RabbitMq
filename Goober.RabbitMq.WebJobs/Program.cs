namespace Goober.RabbitMq.WebJobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Goober.Web.ProgramUtils.RunWebhost<Startup>(args);
        }

    }
}
