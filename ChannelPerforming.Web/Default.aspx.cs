using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using ChannelPerforming.Data;
using ChannelPerforming.Entities;
using ChannelPerforming.Common;

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
            List<Media> result =
                _mediaRepository.Get().Where(m => m.MediaProgressStateType == Utils.MediaProgressStateTypeComplete).
                    ToList();
            ListViewVideoList.DataSource = result;
            ListViewVideoList.DataBind();
        }
    }
}