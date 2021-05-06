using Goober.Core.Services;
using Goober.RabbitMq.Api.Models;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using NSubstitute;
using System;
using System.Linq;
using Goober.RabbitMq.DAL.Repositories;

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
                urlPath: TestUtils.RegisterMessageUrlPath,
                request: registerRequest);

            //assert
            Assert.NotNull(res);
            Assert.NotNull(res.Hash);
            Assert.NotEqual(expected: default(Guid), actual: res.Id);

            var dbResult = await sut.GetRequiredService<IMessageRepository>().GetByIdAsync(res.Id);
            Assert.NotNull(dbResult);
            Assert.NotNull(dbResult.DateOfDelete);

            Assert.Equal(expected: registerRequest.CheckTransactionUrl, actual: dbResult.CheckTransactionUrl);
            Assert.Equal(expected: registerRequest.JMessage, actual: dbResult.JMessage);
            Assert.Equal(expected: registerRequest.MessageTypeFullName, actual: dbResult.MessageTypeFullName);
            Assert.Equal(expected: registerRequest.ProducerApplicationName, actual: dbResult.ProducerApplicationName);
            Assert.Equal(expected: registerRequest.ProducerCallerMemberName, actual: dbResult.ProducerCallerMemberName);
            Assert.Equal(expected: registerRequest.ProducerHost, actual: dbResult.ProducerHost);
        }

        [Fact]
        public async Task RegisterSameMessageTwice_WithoutFirstMessagePublished_ShouldNotCreateSecondAndReturnFirst()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var currentDateTime = sut.CreateFixture<DateTime>();

            sut.GetRequiredService<IDateTimeService>()
                .GetDateTimeNow()
                .Returns(currentDateTime);

            var registerFirst = sut.CreateFixture<RegisterMessageRequest>();

            var registerSecond = sut.BuildFixture<RegisterMessageRequest>()
                .With(x => x.JMessage, registerFirst.JMessage)
                .With(x => x.MessageTypeFullName, registerFirst.MessageTypeFullName)
                .Create();

            //act
            var firstResult = await sut.ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                urlPath: TestUtils.RegisterMessageUrlPath,
                request: registerFirst);

            var secondResult = await sut.ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                urlPath: TestUtils.RegisterMessageUrlPath,
                request: registerSecond);

            //assert
            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.Equal(expected: firstResult.Id, actual: secondResult.Id);
            Assert.Equal(expected: firstResult.Hash, actual: secondResult.Hash);

            var dbRecords = await sut.GetRequiredService<IMessageRepository>().GetAllForTestAsync();
            Assert.Single(dbRecords);
            var singleDbRecord = dbRecords.Single();

            Assert.Equal(expected: firstResult.Id, actual: singleDbRecord.Id);
        }

        [Fact]
        public async Task RegisterSameMessageTwice_FirstMessagePublished_ShouldCreateNew()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var currentDateTime = sut.CreateFixture<DateTime>();

            sut.GetRequiredService<IDateTimeService>()
                .GetDateTimeNow()
                .Returns(currentDateTime);

            var registerFirst = sut.CreateFixture<RegisterMessageRequest>();

            var registerSecond = sut.BuildFixture<RegisterMessageRequest>()
                .With(x => x.JMessage, registerFirst.JMessage)
                .With(x => x.MessageTypeFullName, registerFirst.MessageTypeFullName)
                .Create();

            //act
            var firstResult = await sut.ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                urlPath: TestUtils.RegisterMessageUrlPath,
                request: registerFirst);

            var firstPublishedResult = await sut.ExecutePostAsync<SetPublishedMessageResponse, SetPublishedMessageRequest>(
                urlPath: TestUtils.SetPublishedMessageUrlPath,
                request: new SetPublishedMessageRequest { Id = firstResult.Id });

            var secondResult = await sut.ExecutePostAsync<RegisterMessageResponse, RegisterMessageRequest>(
                urlPath: TestUtils.RegisterMessageUrlPath,
                request: registerSecond);

            //assert
            Assert.NotEqual(firstResult.Id, secondResult.Id);

            var dbRecords = await sut.GetRequiredService<IMessageRepository>().GetAllForTestAsync();
            Assert.Equal(expected: 2, actual: dbRecords.Count);
            var firstDbRecord = dbRecords.First();
            var secondDbRecord = dbRecords.Skip(1).First();
            Assert.Equal(expected: firstResult.Id, actual: firstDbRecord.Id);
            Assert.NotNull(firstDbRecord.PublishedDateTime);
            Assert.Equal(expected: secondResult.Id, actual: secondDbRecord.Id);
        }
    }
}
