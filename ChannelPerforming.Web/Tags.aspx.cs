namespace ChannelPerforming.Web
{
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;
    using ChannelPerforming.Web.ViewDatas;
    using ChannelPerforming.Common;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    public partial class Tags : Page
    {
        private readonly ChannelPerformingRepository<Tag> _tagRepository = new ChannelPerformingRepository<Tag>();
        private readonly ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();
        protected void Page_Load(object sender, EventArgs e)
        {
            string tag = Request.QueryString["tag"];
            if (!string.IsNullOrEmpty(tag))
            {
                Page.Title = tag;
                Load(tag);
            }
        }

        private void Load(string tags)
        {
            List<Tag> result = _tagRepository.Get().Where(t => t.TagName.Equals(tags, StringComparison.OrdinalIgnoreCase)).ToList();
            List<MediaViewData> temp = new List<MediaViewData>();
            if (tags != null)
            {
                foreach (Tag t in result)
                {
                    MediaViewData m = MediaIdbyMediaViewData(t.MediaRowKey);
                    if (m != null)
                    {
                        temp.Add(m);
                    }
                }
            }

            VideoList.DataSource = temp;
            VideoList.DataBind();
        }

        public MediaViewData MediaIdbyMediaViewData(string rowkey)
        {
            Media media = _mediaRepository.Get()
                .Where(m => m.MediaProgressStateType == Utils.MediaProgressStateTypeComplete).ToList()
                .Where(m => m.RowKey == rowkey).FirstOrDefault();

            if (media != null)
            {
                return new MediaViewData()
                           {
                               ThumbnailImageUrl = media.ThumbnailImageUrl,
                               MediaUrl = media.MediaUrl,
                               Description = media.Description,
                               RowKey = media.RowKey,
                               Title = media.Title,
                               MediaProgressStateType = media.MediaProgressStateType,
                               Rating = media.Rating
                           };
            }

            return null;
        }
    }
}