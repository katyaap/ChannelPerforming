namespace ChannelPerforming.Entities
{
    public class Media : EntityBase
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public long Rating { get; set; }

        public string MediaProgressStateType { get; set; }

        public string MediaUrl { get; set; }

        public string ThumbnailImageUrl { get; set; }
    }
}
