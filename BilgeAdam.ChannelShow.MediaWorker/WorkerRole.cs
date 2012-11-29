using System.Collections.Generic;
using ChannelPerforming.Common;
using ChannelPerforming.Data;
using ChannelPerforming.Entities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

using System;
using System.Net;
using System.IO;
using System.Threading;

namespace BilgeAdam.ChannelShow.MediaWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static CloudQueue queue;
        private static CloudBlobContainer container;

        public override void Run()
        {
            Thread thread = new Thread(new ThreadStart(Fill));
            thread.Start();
        }

        private void Fill()
        {
            while (true)
            {
                try
                {
                    CloudQueueMessage message = queue.GetMessage();
                    if (message != null)
                    {
                        string[] messageArray = message.AsString.Split(',');
                        string bloburl = messageArray[0];
                        string partitionKey = messageArray[1];
                        string rowKey = messageArray[2];

                        ChannelPerformingRepository<Media> channelShowContext = new ChannelPerformingRepository<Media>();

                        Media media = channelShowContext.Find(partitionKey, rowKey);

                        media.MediaProgressStateType = Utils.MediaProgressStateTypeWait;
                        channelShowContext.Update(media);
                        channelShowContext.SubmitChange();

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

                        channelShowContext.Update(media);
                        channelShowContext.SubmitChange();

                        queue.Delete();
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)));


            var storageAccount = CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);

            CloudQueueClient queueStorage = storageAccount.CreateCloudQueueClient();
            queue = queueStorage.GetQueueReference(Utils.CloudQueueKey);
            queue.CreateIfNotExist();

            return base.OnStart();
        }
    }
}
