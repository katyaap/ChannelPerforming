using System;
using Microsoft.WindowsAzure.StorageClient;

namespace ChannelPerforming.Entities
{
    public class EntityBase : TableServiceEntity
    {
        public EntityBase()
        {
            PartitionKey = DateTime.UtcNow.ToString("MMddyyyy");
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }
    }
}
