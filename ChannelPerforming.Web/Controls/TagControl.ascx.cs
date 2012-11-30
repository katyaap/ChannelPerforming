using System.Collections.Generic;

namespace ChannelPerforming.Web.Controls
{
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;

    using System;
    using System.Text;
    using System.Web.UI;
    using System.Linq;

    public partial class TagControl : UserControl
    {
        private readonly ChannelPerformingRepository<Tag> _tagRepository = new ChannelPerformingRepository<Tag>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Load();
        }

        private void Load()
        {
            List<string> tags = new List<string>();

            foreach (var tag in _tagRepository.Get().ToList())
            {
                tags.Add(tag.TagName);
            }

            StringBuilder builder = new StringBuilder();
            foreach (var tag in tags.Distinct())
            {
                builder.Append(string.Format("<a href='/Tags.aspx?tag={0}'>{1}</a>", tag, tag));
            }

            LiteralTag.Text = builder.ToString();
        }
    }
}