namespace ChannelPerforming.MediaWorker
{
    using ChannelPerforming.Common;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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

            task.OutputAssets.AddNew("Output asset", true);

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

            task.OutputAssets.AddNew("Output asset", true);

            job.Submit();

            job = GetJob(job.Id);

            IAsset outputAsset = job.OutputMediaAssets[0];
            // IAccessPolicy policy = null;
            // ILocator locator = null;

            //policy = GetCloudMediaContext().AccessPolicies.Create("My 30 days readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            //locator = GetCloudMediaContext().Locators.CreateLocator(LocatorType.Sas, outputAsset,
            //    policy,
            //    DateTime.UtcNow.AddMinutes(-5));

            while (GetJob(job.Id).State != JobState.Finished)
            {

            }

            return outputAsset.Id;

            //List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);

            //if (sasUrlList == null)
            //{
            //    return string.Empty;
            //}

            //if (sasUrlList.Count == 0)
            //{
            //    return string.Empty;
            //}

            //return GetVideoUrl(sasUrlList);
        }

        public static string CreatePlayReadyProtectionJob(string inputMediaFilePath)
        {
            // Create a storage-encrypted asset and upload the mp4. 
            IAsset asset = CreateAssetAndUploadSingleFile(AssetCreationOptions.StorageEncrypted, inputMediaFilePath);

            // Declare a new job to contain the tasks.
            IJob job = _mediaContext.Jobs.Create("My PlayReady Protection job");

            // Set up the first task. 

            // Read the task configuration data into a string. 


            string configMp4ToSmooth = File.ReadAllText(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "MediaPackager_MP4ToSmooth.xml"));
            // Get a media processor reference, and pass to it the name of the 
            // processor to use for the specific task.
            IMediaProcessor processor = GetLatestMediaProcessorByName("Windows Azure Media Packager");

            // Create a task with the conversion details, using the configuration data. 
            ITask task = job.Tasks.AddNew("My Mp4 to Smooth Task",
                processor,
                configMp4ToSmooth,
                TaskOptions.None);
            // Specify the input asset to be converted.
            task.InputAssets.Add(asset);

            // Add an output asset to contain the results of the job. We do not need 
            // to persist the output asset to storage, so set the shouldPersistOutputOnCompletion
            // param to false. 

            task.OutputAssets.AddNew("Streaming output asset_" + DateTime.Now.ToString("hhmmss"), true);

            IAsset smoothOutputAsset = task.OutputAssets[0];

            // Set up the second task. 

            // Read the configuration data into a string. 
            string configPlayReady = File.ReadAllText(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "MediaEncryptor_PlayReadyProtection.xml"));

            // Get a media processor reference, and pass to it the name of the 
            // processor to use for the specific task.
            IMediaProcessor playreadyProcessor = GetLatestMediaProcessorByName("Windows Azure Media Encryptor");

            // Create a second task. 
            ITask playreadyTask = job.Tasks.AddNew("My PlayReady Task_" + DateTime.Now.ToString("hhmmss"),
                playreadyProcessor,
                configPlayReady,
                TaskOptions.ProtectedConfiguration);

            // Add the input asset, which is the smooth streaming output asset from the first task. 
            playreadyTask.InputAssets.Add(smoothOutputAsset);
            // Add an output asset to contain the results of the job. Persist the output by setting 
            // the shouldPersistOutputOnCompletion param to true.
            playreadyTask.OutputAssets.AddNew("PlayReady protected output asset_" + DateTime.Now.ToString("hhmmss"), true);

            // Launch the job.
            job.Submit();

            IAsset outputAsset = job.OutputMediaAssets[0];
            //IAccessPolicy policy = null;
            //ILocator locator = null;

            //policy = GetCloudMediaContext().AccessPolicies.Create("My 30 days readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            //locator = GetCloudMediaContext().Locators.CreateLocator(LocatorType.Sas, outputAsset, policy, DateTime.UtcNow.AddMinutes(-5));


            while (GetJob(job.Id).State != JobState.Finished)
            {
            }

            return outputAsset.Id;

            //List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);

            //if (sasUrlList == null)
            //{
            //    return string.Empty;
            //}

            //if (sasUrlList.Count == 0)
            //{
            //    return string.Empty;
            //}

            //return GetVideoPlayReadyProtectionUrl(sasUrlList);
        }


        private static string GetVideoUrl(List<string> urls)
        {
            foreach (string url in urls)
            {
                if (url.Split('?').Any())
                {
                    string path = url.Split('?')[0];
                    if (Path.GetExtension(path).Equals(".mp4", StringComparison.OrdinalIgnoreCase))
                    {
                        return url;
                    }
                }
            }

            return string.Empty;
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
            var jobInstance = from j in GetCloudMediaContext().Jobs
                              where j.Id == jobId
                              select j;

            IJob job = jobInstance.FirstOrDefault();

            return job;
        }
    }
}
