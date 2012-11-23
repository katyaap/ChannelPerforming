using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using ChannelPerforming.Data;
using ChannelPerforming.Entities;

namespace ChannelPerforming.Web.Manager
{
    public partial class VideoProgressListPage : Page
    {
        private readonly ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Load();
        }

        private void Load()
        {
            List<Media> result = _mediaRepository.Get().ToList().OrderByDescending(m => m.Timestamp).ToList();
            GridViewVideo.DataSource = result;
            GridViewVideo.DataBind();
        }
    }
}