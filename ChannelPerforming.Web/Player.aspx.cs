namespace ChannelPerforming.Web
{
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    public partial class Player : Page
    {

        public static string ThumbnailImageUrl;
        public static string MediaUrl;
        public static string Description;
        public static string Title;
        public static string RowKey;

        private ChannelPerformingRepository<Media> _mediaRepository = new ChannelPerformingRepository<Media>();
        private ChannelPerformingRepository<Comment> _commentRepository = new ChannelPerformingRepository<Comment>();
        protected void Page_Load(object sender, EventArgs e)
        {
            string queryString = Request.QueryString["q"];
            if (!string.IsNullOrEmpty(queryString))
            {
                Media media = _mediaRepository.Find(queryString);
                Page.Title = media.Title;
                ThumbnailImageUrl = media.ThumbnailImageUrl;
                MediaUrl = media.MediaUrl;
                Title = media.Title;
                Description = media.Description;
                RowKey = media.RowKey;
                HiddenFieldVideo.Value = media.RowKey;
                LoadComment(media.RowKey);
            }
        }

        protected void ButtonSend_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                _commentRepository.Create(new Comment
                                              {
                                                  MediaRowKey = HiddenFieldVideo.Value,
                                                  Contnet = TextBoxCommentContent.Text,
                                                  Email = TextBoxCommentEmail.Text,
                                                  RecordDate = DateTime.UtcNow,
                                                  UserName = TextBoxCommentName.Text
                                              });
                _commentRepository.SubmitChange();
                LabelCommentResult.Text = "Comment Sended";
            }
        }

        private void LoadComment(string mediaRow)
        {
            List<Comment> comments = _commentRepository.Get().Where(c => c.MediaRowKey == mediaRow).ToList();
            DataListComment.DataSource = comments;
            DataListComment.DataBind();
        }
    }
}