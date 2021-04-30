using Goober.Tests;
using System.Collections.Generic;

namespace Goober.RabbitMq.WebApi.Tests
{
    public static class TestUtils
    {
        public static SystemUnderTest GenerateSut()
        {
            var ret = new SystemUnderTest(new List<string> { });

            ret.Init<Goober.RabbitMq.WebApi.Startup>(services =>
            {
                ret.RegisterInMemoryDatabase<Goober.RabbitMq.DAL.MsSql.DbContext.Implementation.RabbitMqDBContext>(services);
            });

            return ret;

        }

        public const string RegisterMessageUrlPath = "/api/message/register";

        public const string SetPublishedMessageUrlPath = "/api/message/set-published";
    }
}
