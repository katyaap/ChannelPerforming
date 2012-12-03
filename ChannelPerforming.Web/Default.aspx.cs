using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using ChannelPerforming.Data;
using ChannelPerforming.Entities;
using ChannelPerforming.Common;
using ChannelPerforming.Web.ViewDatas;

namespace ChannelPerforming.Web
{
    public partial class _Default : Page
    {
        private readonly ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Load();
        }

        private void Load()
        {
            List<MediaViewData> views = new List<MediaViewData>();

            _mediaRepository.Get().Where(m => m.MediaProgressStateType == Utils.MediaProgressStateTypeComplete).
                ToList().OrderByDescending(m => m.Timestamp).ToList().ForEach(x => views.Add(new MediaViewData()
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
        }
    }
}