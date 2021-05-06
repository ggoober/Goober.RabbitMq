using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goober.RabbitMq.DAL.MsSql.Entities
{
    [Table("Messages")]
    internal class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Hash { get; set; }

        public string MessageTypeFullName { get; set; }

        public string JMessage { get; set; }

        public string CheckTransactionUrl { get; set; }

        public string ProducerCallerMemberName { get; set; }

        public string ProducerApplicationName { get; set; }

        public string ProducerHost { get; set; }

        public DateTime? PublishedDateTime { get; set; }

        public DateTime? DateOfDelete { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RowVersion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RowCreatedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RowChangedDate { get; set; }

        public DateTime? ConcurrentSelectLockDateTime { get; set; }
    }
}
