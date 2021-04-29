using System;

namespace Goober.RabbitMq.DAL.Models
{
    public class MessageModel
    {
        public long Id { get; set; }

        public string Hash { get; set; }

        public string MessageTypeFullName { get; set; }

        public string JMessage { get; set; }

        public string CheckTransactionUrl { get; set; }

        public string ProducerCallerMemberName { get; set; }

        public string ProducerApplicationName { get; set; }

        public string ProducerHost { get; set; }

        public DateTime? PublishedDateTime { get; set; }

        public string RowVersion { get; set; }

        public DateTime RowCreatedDate { get; set; }

        public DateTime RowChangedDate { get; set; }
    }
}
