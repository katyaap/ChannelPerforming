namespace ChannelPerforming.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.UI;

    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;
    
    using Microsoft.WindowsAzure.MediaServices.Client;

    public partial class ProtectionPlayer : Page
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
                MediaUrl = GetVideo(media.AssetId);
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

        private string GetVideo(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                List<String> sasUrlList;
                CloudMediaContext mediaContext = Global.GetCloudMediaContext();

                IAsset asset = mediaContext.Assets.Where(x => x.Id == assetId).FirstOrDefault();
                if (asset.Locators.Count == 0)
                {
                    IAccessPolicy policy = null;
                    ILocator locator = null;

                    policy = mediaContext.AccessPolicies.Create("My 30 days readonly policy", TimeSpan.FromHours(1), AccessPermissions.Read);

                    locator = mediaContext.Locators.CreateLocator(LocatorType.Sas, asset, policy, DateTime.UtcNow.AddMinutes(-5));

                    sasUrlList = GetAssetSasUrlList(asset, locator);

                    if (sasUrlList == null)
                    {
                        return string.Empty;
                    }

                    if (sasUrlList.Count == 0)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    asset.Locators.First().ExpirationDateTime.AddMinutes(30);
                    sasUrlList = GetAssetSasUrlList(asset, asset.Locators.First());
                }

                return GetVideoUrl(sasUrlList);
            }

            return string.Empty;
        }

        private static string GetVideoUrl(List<string> urls)
        {
            foreach (string url in urls)
            {
                if (url.Split('?').Any())
                {
                    string path = url.Split('?')[0];
                    if (Path.GetExtension(path).Equals(".ism", StringComparison.OrdinalIgnoreCase))
                    {
                        return string.Format("{0}/Manifest", url);
                        return url;
                    }
                }
            }

            return string.Empty;
        }

        private static List<string> GetAssetSasUrlList(IAsset asset, ILocator locator)
        {
            List<String> fileSasUrlList = new List<String>();

            foreach (var file in asset.AssetFiles)
            {
                file.IsPrimary = true;
                if (file.IsEncrypted)
                {
                    
                }


                string sasUrl = BuildFileSasUrl(file.Name, locator);
                fileSasUrlList.Add(sasUrl);
            }

            return fileSasUrlList;
        }

        private static string BuildFileSasUrl(string fileName, ILocator locator)
        {
            var uriBuilder = new UriBuilder(locator.Path);
            uriBuilder.Path += "/" + fileName;

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}