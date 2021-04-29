using Goober.Tests;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
