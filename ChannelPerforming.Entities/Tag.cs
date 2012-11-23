using System;

namespace ChannelPerforming.Entities
{
    public class Tag : EntityBase
    {
        public string TagName { get; set; }

        public string MediaPartitionKey { get; set; }

        public string MediaRowKey { get; set; }

        public DateTime RecordDate { get; set; }
    }
}
