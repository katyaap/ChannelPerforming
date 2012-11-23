using System;
using System.Web.UI;

using ChannelPerforming.Data;
using ChannelPerforming.Entities;

namespace ChannelPerforming.Web
{
    public partial class VideoPlayer : Page
    {
        public static string ThumbnailImageUrl;
        public static string MediaUrl;
        public static string Description;
        public static string Title;
        public static string RowKey;

        private ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string queryString = Request.QueryString["q"];
            if (!string.IsNullOrEmpty(queryString))
            {
                Media media = _mediaRepository.Find(queryString);
                ThumbnailImageUrl = media.ThumbnailImageUrl;
                MediaUrl = media.MediaUrl;
                Title = media.Title;
                Description = media.Description;
                RowKey = media.RowKey;
            }
        }
    }
}