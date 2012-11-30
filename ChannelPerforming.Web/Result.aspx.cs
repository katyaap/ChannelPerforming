namespace ChannelPerforming.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    using ChannelPerforming.Common;
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;
    using ChannelPerforming.Web.ViewDatas;

    public partial class Result : Page
    {
        private readonly ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string queryString = Request.QueryString["q"];

            if (!string.IsNullOrEmpty(queryString))
            {
                Load(queryString);
            }
        }

        private void Load(string query)
        {
            List<MediaViewData> views = new List<MediaViewData>();

            _mediaRepository.Get().Where(m => m.MediaProgressStateType == Utils.MediaProgressStateTypeComplete)
                .ToList().Where(m => m.Title.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => views.Add(new MediaViewData()
                {
                    Description = x.Description,
                    RowKey = x.RowKey,
                    Rating = x.Rating,
                    MediaProgressStateType = x.MediaProgressStateType,
                    MediaUrl = x.MediaUrl,
                    ThumbnailImageUrl = x.ThumbnailImageUrl,
                    Title = x.Title
                }));

            VideoList.DataSource = views;
            VideoList.DataBind();
            Page.Title = query;
        }
    }
}