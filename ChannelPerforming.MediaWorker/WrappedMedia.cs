namespace ChannelPerforming.MediaWorker
{
    using ChannelPerforming.Common;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WrappedMedia
    {
        private static CloudMediaContext _mediaContext;

        private static CloudMediaContext GetCloudMediaContext()
        {
            string accoutName = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutName);
            string accoutKey = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutKey);

            if (_mediaContext == null)
            {
                _mediaContext = new CloudMediaContext(accoutName, accoutKey);
            }

            return _mediaContext;
        }

        public static string CreateThumbnailTask(string inputFilePath)
        {
            string xmlPreset = File.ReadAllText(Path.GetFullPath(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "Thumbnail.xml")));
            IAsset asset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, inputFilePath);
            IJob job = GetCloudMediaContext().Jobs.Create("My Thumbnail job");

            IMediaProcessor processor = GetLatestMediaProcessorByName("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew("My Thumbnail job", processor, xmlPreset, TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);

            task.OutputAssets.AddNew("Output asset",
                true,
                AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);

            IAsset outputAsset = job.OutputMediaAssets[0];
            IAccessPolicy policy = null;
            ILocator locator = null;

            policy = GetCloudMediaContext().AccessPolicies.Create("My one-hour readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            locator = GetCloudMediaContext().Locators.CreateLocator(LocatorType.Sas, outputAsset,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            while (GetJob(job.Id).State != JobState.Finished)
            {

            }

            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);

            if (sasUrlList.Count > 0)
            {
                return sasUrlList[0];
            }

            return string.Empty;
        }

        public static string CreateEncodingJob(string inputMediaFilePath)
        {
            IAsset asset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, inputMediaFilePath);

            IJob job = GetCloudMediaContext().Jobs.Create("My encoding job");

            IMediaProcessor processor = GetLatestMediaProcessorByName("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew("My encoding task", processor, "H264 Broadband 720p", TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);


            task.OutputAssets.AddNew("Output asset",
                true,
                AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);

            IAsset outputAsset = job.OutputMediaAssets[0];
            IAccessPolicy policy = null;
            ILocator locator = null;

            policy = GetCloudMediaContext().AccessPolicies.Create("My 30 days readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            locator = GetCloudMediaContext().Locators.CreateLocator(LocatorType.Sas, outputAsset,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            while (GetJob(job.Id).State != JobState.Finished)
            {

            }

            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);

            if (sasUrlList == null)
            {
                return string.Empty;
            }

            if (sasUrlList.Count == 0)
            {
                return string.Empty;
            }

            return sasUrlList[0];
        }

        private static IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string singleFilePath)
        {
            string assetName = "UploadSingleFile_" + DateTime.UtcNow.ToString();
            IAsset asset = CreateEmptyAsset(assetName, assetCreationOptions);

            string fileName = Path.GetFileName(singleFilePath);

            IAssetFile assetFile = asset.AssetFiles.Create(fileName);

            assetFile.Upload(singleFilePath);

            return asset;
        }

        private static IAsset CreateEmptyAsset(string assetName, AssetCreationOptions assetCreationOptions)
        {
            return GetCloudMediaContext().Assets.Create(assetName, assetCreationOptions);
        }

        private static List<string> GetAssetSasUrlList(IAsset asset, ILocator locator)
        {
            List<String> fileSasUrlList = new List<String>();

            foreach (var file in asset.AssetFiles)
            {
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

        private static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = GetCloudMediaContext().MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        private static IJob GetJob(string jobId)
        {
            Thread.Sleep(500);
            var jobInstance = from j in GetCloudMediaContext().Jobs
                              where j.Id == jobId
                              select j;

            IJob job = jobInstance.FirstOrDefault();

            return job;
        }
    }
}
