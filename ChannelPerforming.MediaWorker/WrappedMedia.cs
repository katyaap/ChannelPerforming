namespace ChannelPerforming.MediaWorker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ChannelPerforming.Common;

    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.ServiceRuntime;

    internal class WrappedMedia
    {
        private static readonly CloudMediaContext _mediaContext;

        static WrappedMedia()
        {
            string accoutName = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutName);
            string accoutKey = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutKey);

            _mediaContext = new CloudMediaContext(accoutName, accoutKey);
        }

        public static string CreateEncodingJob(string inputMediaFilePath)
        {
            IAsset asset = _mediaContext.Assets.Create(inputMediaFilePath,
                AssetCreationOptions.StorageEncrypted);

            IJob job = _mediaContext.Jobs.Create("My encoding job");

            IMediaProcessor processor = GetMediaProcessor("Windows Azure Media Encoder");

            string configuration = File.ReadAllText(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "MP4 to Smooth Streams.xml"));
            ITask task = job.Tasks.AddNew("My encoding task",
                processor,
                configuration,
                TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew("Output asset", true, AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);

            IAsset outputAsset = job.OutputMediaAssets[0];

            IAccessPolicy thePolicy = _mediaContext.AccessPolicies.Create("My one-hour readonly policy", TimeSpan.FromHours(1), AccessPermissions.Read);
            ILocator locator = _mediaContext.Locators.CreateSasLocator(outputAsset, thePolicy, DateTime.UtcNow.AddMinutes(-5));

            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);
            if (sasUrlList.Count == 0)
            {
                return string.Empty;
            }

            return sasUrlList[0];
        }

        public static string CreateThumbnailTask(string inputFilePath)
        {
            string xmlPreset = File.ReadAllText(Path.GetFullPath(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "Thumbnail.xml")));

            IAsset asset = _mediaContext.Assets.Create(inputFilePath, AssetCreationOptions.None);

            IJob job = _mediaContext.Jobs.Create("My Thumbnail job");
            IMediaProcessor processor = GetMediaProcessor("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew("My thumbnail task",
                processor,
                xmlPreset,
                TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew("Output asset", true, AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);
            IAsset outputAsset = job.OutputMediaAssets[0];

            IAccessPolicy thePolicy =
                _mediaContext.AccessPolicies.Create("My one-hour readonly policy",
                    TimeSpan.FromHours(1),
                    AccessPermissions.Read);

            ILocator locator = _mediaContext.Locators.CreateSasLocator(outputAsset,
                thePolicy,
                DateTime.UtcNow.AddMinutes(-5));

            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);
            if (sasUrlList.Count > 0)
            {
                return sasUrlList[0];
            }

            return string.Empty;
        }

        private static IMediaProcessor GetMediaProcessor(string mediaProcessor)
        {
            var theProcessor = from p in _mediaContext.MediaProcessors
                               where p.Name == mediaProcessor
                               select p;

            IMediaProcessor processor = theProcessor.First();

            if (processor == null)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown processor", mediaProcessor));
            }

            return processor;
        }

        private static string BuildFileSasUrl(string fileName, ILocator locator)
        {
            var uriBuilder = new UriBuilder(locator.Path);
            uriBuilder.Path += "/" + fileName;

            return uriBuilder.Uri.AbsoluteUri;
        }

        private static List<String> GetAssetSasUrlList(IAsset asset, ILocator locator)
        {
            List<String> fileSasUrlList = new List<String>();

            foreach (var file in asset.AssetFiles)
            {
                string sasUrl = BuildFileSasUrl(file.Name, locator);
                fileSasUrlList.Add(sasUrl);
            }

            return fileSasUrlList;
        }

        private static IJob GetJob(string jobId)
        {
            var job = from j in _mediaContext.Jobs
                      where j.Id == jobId
                      select j;

            IJob theJob = job.SingleOrDefault();

            if (theJob != null)
            {
                return theJob;
            }

            return null;
        }
    }
}
