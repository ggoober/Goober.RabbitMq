using Goober.Core.Services;
using Goober.RabbitMq.Api.Models;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using System;
using Goober.RabbitMq.DAL.Repository;

namespace Goober.RabbitMq.WebApi.Tests.MessageApiControllerTests
{
    public class RegisterTests
    {
        [Fact]
        public async Task RegisterNewMessage_ShoultCreateIt()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var currentDateTime = sut.CreateFixture<DateTime>();

            sut.GetRequiredService<IDateTimeService>()
                .GetDateTimeNow()
                .Returns(currentDateTime);

            var registerRequest = sut.CreateFixture<RegisterMessageRequest>();

            //act
            var res = await sut.ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                urlPath: "/api/message/register",
                request: registerRequest);

            //assert
            Assert.NotNull(res);
            Assert.NotNull(res.Hash);
            Assert.NotEqual(expected: 0, actual: res.Id);

            var dbResult = await sut.GetRequiredService<IMessageRepository>().GetByIdAsync(res.Id);
            Assert.NotNull(dbResult);
            Assert.Equal(expected: registerRequest.CheckTransactionUrl, actual: dbResult.CheckTransactionUrl);
            Assert.Equal(expected: registerRequest.JMessage, actual: dbResult.JMessage);
            Assert.Equal(expected: registerRequest.MessageTypeFullName, actual: dbResult.MessageTypeFullName);
            Assert.Equal(expected: registerRequest.ProducerApplicationName, actual: dbResult.ProducerApplicationName);
            Assert.Equal(expected: registerRequest.ProducerCallerMemberName, actual: dbResult.ProducerCallerMemberName);
            Assert.Equal(expected: registerRequest.ProducerHost, actual: dbResult.ProducerHost);
        }
    }
}
