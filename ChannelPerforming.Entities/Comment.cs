using System;
using Microsoft.WindowsAzure.StorageClient;

namespace ChannelPerforming.Entities
{
    public class Comment : EntityBase
    {
        public Comment()
        {
            PartitionKey = DateTime.UtcNow.ToString("MMddyyyy");
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }

        public string Contnet { get; set; }

        public string UserName { get; set; }

        public string MediaRowKey { get; set; }

        public DateTime RecordDate { get; set; }

        public string Email { get; set; }
    }
}
