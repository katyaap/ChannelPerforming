namespace ChannelPerforming.MediaWorker
{
    using ChannelPerforming.Common;
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.StorageClient;

    using System;
    using System.IO;
    using System.Net;

    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue _queue;
        private CloudBlobContainer _container;

        public override void Run()
        {
            while (true)
            {
                CloudQueueMessage message = _queue.GetMessage();
                if (message != null)
                {
                    string[] messageArray = message.AsString.Split(',');
                    string bloburl = messageArray[0];
                    string partitionKey = messageArray[1];
                    string rowKey = messageArray[2];

                    ChannelPerformingRepository<Media> repository = new ChannelPerformingRepository<Media>();

                    Media media = repository.Find(partitionKey, rowKey);

                    media.MediaProgressStateType = Utils.MediaProgressStateTypeBegin;
                    repository.Update(media);
                    repository.SubmitChange();

                    string dowlandpath = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("RdRoleRoot"),
                                                       Utils.MediaDownloadFilePath);
                    if (!Directory.Exists(dowlandpath))
                    {
                        Directory.CreateDirectory(dowlandpath);
                    }

                    string dowlandfilepath = Helpers.DownloadAssetToLocal(bloburl, dowlandpath);
                    string thumbnailPath = WrappedMedia.CreateThumbnailTask(dowlandfilepath);
                    string mediaEncodePath = WrappedMedia.CreateEncodingJob(dowlandfilepath);

                    media.MediaProgressStateType = Utils.MediaProgressStateTypeComplete;
                    media.ThumbnailImageUrl = thumbnailPath;
                    media.MediaUrl = mediaEncodePath;

                    repository.Update(media);
                    repository.SubmitChange();

                    _queue.Delete();
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });


            var storageAccount = CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);

            CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
            _container = blobStorage.GetContainerReference(Utils.CloudBlobKey);

            CloudQueueClient queueStorage = storageAccount.CreateCloudQueueClient();
            _queue = queueStorage.GetQueueReference(Utils.CloudQueueKey);


            bool storageInitialized = false;

            while (!storageInitialized)
            {
                try
                {
                    
                    var permissions = _container.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    _container.SetPermissions(permissions);
                    
                    _container.CreateIfNotExist();
                    _queue.CreateIfNotExist();

                    storageInitialized = true;
                }
                catch (StorageClientException e)
                {
                    //logger.Log(LogLevel.Error, e);
                }
            }


            return base.OnStart();
        }
    }
}
