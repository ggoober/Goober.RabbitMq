using AutoFixture;
using NSubstitute;
using Goober.RabbitMq.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Goober.Core.Services;
using Goober.RabbitMq.Api.Models;
using Goober.RabbitMq.DAL.Repositories;
using System.Net;

namespace Goober.RabbitMq.WebApi.Tests.MessageApiControllerTests
{
    public class SetPublishedTests
    {
        [Fact]
        public async Task SetPublished_ForNotPublishedMessage_ShouldSetPublishedAndReturnDate()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var currentDateTime = sut.CreateFixture<DateTime>();

            sut.GetRequiredService<IDateTimeService>()
                .GetDateTimeNow()
                .Returns(currentDateTime);

            var existedMessage = sut.BuildFixture<MessageModel>()
                .Without(x=>x.PublishedDateTime)
                .Create();

            await sut.GetRequiredService<IMessageRepository>()
                .InsertAsync(existedMessage);

            //act
            var res = await sut.ExecutePostAsync<SetPublishedMessageResponse, SetPublishedMessageRequest>(
                urlPath: TestUtils.SetPublishedMessageUrlPath,
                request: new SetPublishedMessageRequest { Id = existedMessage.Id, RowVersion = existedMessage.RowVersion }
                );

            //assert
            Assert.Equal(expected: currentDateTime, res.PublishedDateTime);
        }

        [Fact]
        public async Task SetPublished_ForAlreadyPublishedMessage_ShouldReturnExistedPublishedDate()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var existedMessage = sut.BuildFixture<MessageModel>()
                .With(x => x.PublishedDateTime, sut.CreateFixture<DateTime>())
                .Create();

            await sut.GetRequiredService<IMessageRepository>()
                .InsertAsync(existedMessage);

            //act
            var res = await sut.ExecutePostAsync<SetPublishedMessageResponse, SetPublishedMessageRequest>(
                urlPath: TestUtils.SetPublishedMessageUrlPath,
                request: new SetPublishedMessageRequest { Id = existedMessage.Id, RowVersion = existedMessage.RowVersion }
                );

            //assert
            Assert.Equal(expected: existedMessage.PublishedDateTime, res.PublishedDateTime);
        }

        [Fact]
        public async Task SetPublishedWithWrongMessageId_ThrowsException()
        {
            var sut = TestUtils.GenerateSut();

            //arrange
            var request = sut.CreateFixture<SetPublishedMessageRequest>();

            //act + assert
            await Assert.ThrowsAsync<WebException>(() => sut.ExecutePostAsync<SetPublishedMessageResponse, SetPublishedMessageRequest>(
                urlPath: TestUtils.SetPublishedMessageUrlPath,
                request: request));
        }
    }
}
