using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChannelPerforming.Data;
using ChannelPerforming.Entities;

namespace ChannelPerforming.Web
{
    public partial class Player : System.Web.UI.Page
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