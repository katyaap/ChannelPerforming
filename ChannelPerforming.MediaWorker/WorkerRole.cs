namespace ChannelPerforming.MediaWorker
{
    using ChannelPerforming.Common;
    using ChannelPerforming.Data;
    using ChannelPerforming.Entities;

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.StorageClient;

    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue _queue;

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

                    media.MediaProgressStateType = Utils.MediaProgressStateTypeProgress;
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

                    try
                    {
                        repository.Update(media);
                        repository.SubmitChange();
                        _queue.Delete();
                    }
                    catch (Exception e)
                    {
                        Trace.Write(e);
                    }
                }
            }
        }

        public override bool OnStart()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });

            ServicePointManager.DefaultConnectionLimit = 12;

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);

            CloudQueueClient queueStorage = storageAccount.CreateCloudQueueClient();
            _queue = queueStorage.GetQueueReference(Utils.CloudQueueKey);

            _queue.CreateIfNotExist();

            return base.OnStart();
        }
    }
}
