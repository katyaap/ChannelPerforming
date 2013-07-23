namespace ChannelPerforming.Web.Manager
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web.UI;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    using ChannelPerforming.Common;
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;

    public partial class UploadPage : Page
    {
        private static readonly object _look = new object();
        private static bool storageInitialized = false;
        private static CloudBlobClient blobClient;
        private static CloudQueueClient queueClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Video Upload";
            if (IsPostBack) return;
            InitializeStorage();
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            if (VideoUpload.HasFiles & Page.IsValid)
            {
                ChannelPerformingRepository<Media> mediaRepository = new ChannelPerformingRepository<Media>();
                ChannelPerformingRepository<Tag> tagRepository = new ChannelPerformingRepository<Tag>();

                string uniqueBobName = string.Format("{0}/ShowVideo_{1}{2}", Utils.CloudBlobKey, Guid.NewGuid().ToString(), Path.GetExtension(VideoUpload.FileName));
                CloudBlockBlob blob = blobClient.GetBlockBlobReference(uniqueBobName);
                blob.Properties.ContentType = VideoUpload.PostedFile.ContentType;
                blob.UploadFromStream(VideoUpload.FileContent);

                Media media = new Media
                {
                    Description = TextBoxVideoDescription.Text,
                    Title = TextBoxVideoName.Text
                };

                string[] tagArray = TextBoxTags.Text.Split(';');
                foreach (string t in tagArray)
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        Tag tag = new Tag
                        {
                            TagName = t,
                            MediaPartitionKey = media.PartitionKey,
                            MediaRowKey = media.RowKey,
                            RecordDate = DateTime.Now
                        };

                        tagRepository.Create(tag);
                        tagRepository.SubmitChange();
                    }
                }

                media.MediaProgressStateType = Utils.MediaProgressStateTypeWait;
                mediaRepository.Create(media);
                mediaRepository.SubmitChange();

                CloudQueue queue = queueClient.GetQueueReference(Utils.CloudQueueKey);
                CloudQueueMessage message = new CloudQueueMessage(string.Format("{0},{1},{2}", blob.Uri, media.PartitionKey, media.RowKey));
                queue.AddMessage(message);

                LabelResult.Text = "Video Uploaded";
                Response.Redirect("/Manager/VideoProgressListPage.aspx");
            }
        }

        private void InitializeStorage()
        {
            if (storageInitialized)
            {
                return;
            }

            lock (_look)
            {
                if (storageInitialized)
                {
                    return;
                }

                try
                {
                    // read account configuration settings
                    CloudStorageAccount storageAccount = CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);

                    // create blob container for images
                    blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(Utils.CloudBlobKey);
                    container.CreateIfNotExist();

                    // configure container for public access
                    var permissions = container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    container.SetPermissions(permissions);

                    // create queue to communicate with worker role
                    queueClient = storageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueClient.GetQueueReference(Utils.CloudQueueKey);
                    queue.CreateIfNotExist();
                }
                catch (WebException exception)
                {
                    Trace.Write(exception.Message);
                }

                storageInitialized = true;
            }
        }
    }
}