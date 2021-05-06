using AutoFixture;
using NSubstitute;
using Goober.Core.Services;
using Goober.RabbitMq.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Goober.RabbitMq.DAL.Repositories;
using Goober.RabbitMq.Api.Models;
using System.Threading.Tasks;

namespace Goober.RabbitMq.WebApi.Tests.MessageApiControllerTests
{
    public class GetConcurrentWithoutHostNotPublishedIdsTests
    {
        [Fact]
        public async Task T()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var currentDateTime = sut.CreateFixture<DateTime>();
            var baseDelayInMilliseconds = 60000;
            var applicationName = sut.CreateFixture<string>();

            sut.GetRequiredService<IDateTimeService>()
                .GetDateTimeNow()
                .Returns(currentDateTime);

            var message1 = sut.BuildFixture<MessageModel>()
                .With(x => x.ProducerApplicationName, applicationName)
                .Without(x => x.DateOfDelete)
                .Without(x => x.ConcurrentSelectLockDateTime)
                .With(x => x.RowCreatedDate, currentDateTime.AddMilliseconds(-baseDelayInMilliseconds * 6))
                .Create();

            await sut.GetRequiredService<IMessageRepository>()
                .InsertAsync(message1);

            var message2 = sut.BuildFixture<MessageModel>()
                .With(x => x.ProducerApplicationName, applicationName)
                .Without(x => x.DateOfDelete)
                .Without(x => x.ConcurrentSelectLockDateTime)
                .With(x => x.RowCreatedDate, currentDateTime.AddMilliseconds(-baseDelayInMilliseconds * 5))
                .Create();

            await sut.GetRequiredService<IMessageRepository>()
                .InsertAsync(message2);

            sut.GetRequiredService<IMessageRepository>();

            //act
            var res = await sut.ExecutePostAsync<GetConcurrentWithoutHostNotPublishedIdsResponse, GetConcurrentWithoutHostNotPublishedIdsRequest>(
                urlPath: "/api/message/get-concurrent-without-host-not-published-ids",
                request: new GetConcurrentWithoutHostNotPublishedIdsRequest
                {
                    ApplicationName = applicationName,
                    ConcurrentSelectDelayInMilliseconds = baseDelayInMilliseconds * 3,
                    CreatedDelayInMilliseconds = baseDelayInMilliseconds * 1,
                    MessageTypeFullName = null,
                    RowsCount = 100
                }
                );

            //assert
            //Assert.Equal(expected: 1, counter);
            //Assert.Single(res.Ids);

        }
    }
}
