using Goober.EntityFramework.Common.Implementation;
using Goober.RabbitMq.DAL.Models;
using Goober.RabbitMq.DAL.Repository;
using Goober.RabbitMq.DAL.MsSql.DbContext;
using Goober.RabbitMq.DAL.MsSql.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Goober.RabbitMq.DAL.MsSql.Repository.Implementation
{
    class MessageRepository: BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IRabbitMqDBContext dbContext)
            : base(dbContext.Messages)
        {
        }

        public async Task<MessageModel> GetByIdAsync(long id)
        {
            var res = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (res == null)
                return null;
            return ConvertToMessageModel(res);
        }

        private static MessageModel ConvertToMessageModel(Message message)
        {
            return new MessageModel
            {
                Id = message.Id,
                CheckTransactionUrl = message.CheckTransactionUrl,
                Hash = message.Hash,
                JMessage = message.JMessage,
                MessageTypeFullName = message.MessageTypeFullName,
                ProducerApplicationName = message.ProducerApplicationName,
                ProducerCallerMemberName = message.ProducerCallerMemberName,
                ProducerHost = message.ProducerHost,
                PublishedDateTime = message.PublishedDateTime,
                RowChangedDate = message.RowChangedDate,
                RowCreatedDate = message.RowCreatedDate,
                RowVersion = message.RowVersion
            };
        }

        public async Task<MessageModel> GetNotPublishedAsync(string messageTypeFullName, string hash)
        {
            var res = await DbSet.FirstOrDefaultAsync(x => 
                x.MessageTypeFullName == messageTypeFullName 
                && x.Hash == hash 
                && x.PublishedDateTime == null);

            if (res == null)
                return null;

            return ConvertToMessageModel(res);
        }

        public async Task InsertAsync(MessageModel messageModel)
        {
            Message message = ConvertToMessage(messageModel);

            await this.InsertAsync(message);

            messageModel.Id = message.Id;
            messageModel.RowVersion = message.RowVersion;
            messageModel.RowCreatedDate = message.RowCreatedDate;
            messageModel.RowChangedDate = message.RowChangedDate;
        }

        private static Message ConvertToMessage(MessageModel messageModel)
        {
            return new Message
            {
                Id = messageModel.Id,
                MessageTypeFullName = messageModel.MessageTypeFullName,
                RowVersion = messageModel.RowVersion,
                CheckTransactionUrl = messageModel.CheckTransactionUrl,
                Hash = messageModel.Hash,
                JMessage = messageModel.JMessage,
                ProducerApplicationName = messageModel.ProducerApplicationName,
                ProducerCallerMemberName = messageModel.ProducerCallerMemberName,
                ProducerHost = messageModel.ProducerHost,
                PublishedDateTime = messageModel.PublishedDateTime,
                RowChangedDate = messageModel.RowChangedDate,
                RowCreatedDate = messageModel.RowCreatedDate
            };
        }

        public async Task UpdateAsync(MessageModel messageModel)
        {
            var existed = await DbSet.FirstOrDefaultAsync(x => x.Id == messageModel.Id);
            if (existed == null)
            {
                throw new InvalidOperationException($"Can't find message by Id = {messageModel.Id}");
            }

            existed.Hash = messageModel.Hash;
            existed.CheckTransactionUrl = messageModel.CheckTransactionUrl;
            existed.JMessage = messageModel.JMessage;
            existed.MessageTypeFullName = messageModel.MessageTypeFullName;
            existed.ProducerApplicationName = messageModel.ProducerApplicationName;
            existed.ProducerCallerMemberName = messageModel.ProducerCallerMemberName;
            existed.ProducerHost = messageModel.ProducerHost;
            existed.PublishedDateTime = messageModel.PublishedDateTime;
            existed.RowVersion = messageModel.RowVersion;
            existed.RowCreatedDate = messageModel.RowCreatedDate;

            await this.UpdateAsync(existed);

            messageModel.RowVersion = existed.RowVersion;
            messageModel.RowChangedDate = existed.RowChangedDate;
            messageModel.RowCreatedDate = existed.RowCreatedDate;
        }
    }
}
