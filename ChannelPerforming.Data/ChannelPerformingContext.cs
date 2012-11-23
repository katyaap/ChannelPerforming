namespace ChannelPerforming.Data
{
    using System.Linq;

    using ChannelPerforming.Entities;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class ChannelPerformingContext : TableServiceContext
    {
        public ChannelPerformingContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public IQueryable<Comment> Comments
        {
            get
            {
                return this.CreateQuery<Comment>(typeof(Comment).Name);
            }
        }

        public IQueryable<Media> Medias
        {
            get
            {
                return this.CreateQuery<Media>(typeof(Media).Name);
            }
        }

        public IQueryable<Tag> Tags
        {
            get
            {
                return this.CreateQuery<Tag>(typeof(Tag).Name);
            }
        }
    }
}
