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
        private static readonly CloudMediaContext _mediaContext;

        static WrappedMedia()
        {
            string accoutName = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutName);
            string accoutKey = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutKey);

            _mediaContext = new CloudMediaContext(accoutName, accoutKey);
        }

        public static string CreateThumbnailTask(string inputFilePath)
        {
            string xmlPreset = File.ReadAllText(Path.GetFullPath(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "Thumbnail.xml")));
            IAsset asset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, inputFilePath);
            // Declare a new job.
            IJob job = _mediaContext.Jobs.Create("My Thumbnail job");
            // Get a media processor reference, and pass to it the name of the 
            // processor to use for the specific task.
            IMediaProcessor processor = GetLatestMediaProcessorByName("Windows Azure Media Encoder");

            // Create a task with the encoding details, using a string preset.
            ITask task = job.Tasks.AddNew("My Thumbnail job", processor, xmlPreset, TaskOptions.ProtectedConfiguration);

            // Specify the input asset to be encoded.
            task.InputAssets.Add(asset);
            // Add an output asset to contain the results of the job. 
            // This output is specified as AssetCreationOptions.None, which 
            // means the output asset is not encrypted. 
            task.OutputAssets.AddNew("Output asset",
                true,
                AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);

            // Get a reference to the output asset from the job.
            IAsset outputAsset = job.OutputMediaAssets[0];
            IAccessPolicy policy = null;
            ILocator locator = null;

            // Declare an access policy for permissions on the asset. 
            // You can call an async or sync create method. 
            policy = _mediaContext.AccessPolicies.Create("My one-hour readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            // Create a SAS locator to enable direct access to the asset 
            // in blob storage. You can call a sync or async create method.  
            // You can set the optional startTime param as 5 minutes 
            // earlier than Now to compensate for differences in time  
            // between the client and server clocks. 

            locator = _mediaContext.Locators.CreateLocator(LocatorType.Sas, outputAsset,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            // Build a list of SAS URLs to each file in the asset. 
            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);

            if (sasUrlList.Count > 0)
            {
                return sasUrlList[0];
            }

            return string.Empty;
        }

        private static IAsset CreateAssetAndUploadSingleFile(AssetCreationOptions assetCreationOptions, string singleFilePath)
        {
            string assetName = "UploadSingleFile_" + DateTime.UtcNow.ToString();
            IAsset asset = CreateEmptyAsset(assetName, assetCreationOptions);

            string fileName = Path.GetFileName(singleFilePath);

            IAssetFile assetFile = asset.AssetFiles.Create(fileName);

            var accessPolicy = _mediaContext.AccessPolicies.Create(assetName, TimeSpan.FromDays(3), AccessPermissions.Write | AccessPermissions.List);

            ILocator locator = _mediaContext.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);

            assetFile.Upload(singleFilePath);

            return asset;
        }

        public static string CreateEncodingJob(string inputMediaFilePath)
        {
            IAsset asset = CreateAssetAndUploadSingleFile(AssetCreationOptions.None, inputMediaFilePath);
            // Declare a new job.
            IJob job = _mediaContext.Jobs.Create("My encoding job");
            // Get a media processor reference, and pass to it the name of the 
            // processor to use for the specific task.
            IMediaProcessor processor = GetLatestMediaProcessorByName("Windows Azure Media Encoder");

            // Create a task with the encoding details, using a string preset.
            ITask task = job.Tasks.AddNew("My encoding task", processor, "H264 Broadband 720p", TaskOptions.ProtectedConfiguration);

            // Specify the input asset to be encoded.
            task.InputAssets.Add(asset);
            // Add an output asset to contain the results of the job. 
            // This output is specified as AssetCreationOptions.None, which 
            // means the output asset is not encrypted. 
            task.OutputAssets.AddNew("Output asset",
                true,
                AssetCreationOptions.None);

            job.Submit();

            job = GetJob(job.Id);

            // Get a reference to the output asset from the job.
            IAsset outputAsset = job.OutputMediaAssets[0];
            IAccessPolicy policy = null;
            ILocator locator = null;

            // Declare an access policy for permissions on the asset. 
            // You can call an async or sync create method. 
            policy = _mediaContext.AccessPolicies.Create("My 30 days readonly policy", TimeSpan.FromDays(30), AccessPermissions.Read);

            // Create a SAS locator to enable direct access to the asset 
            // in blob storage. You can call a sync or async create method.  
            // You can set the optional startTime param as 5 minutes 
            // earlier than Now to compensate for differences in time  
            // between the client and server clocks. 

            locator = _mediaContext.Locators.CreateLocator(LocatorType.Sas, outputAsset,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            // Build a list of SAS URLs to each file in the asset. 
            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);


            if (sasUrlList == null)
            {
                return string.Empty;
            }

            return sasUrlList[0];
        }

        private static IAsset CreateEmptyAsset(string assetName, AssetCreationOptions assetCreationOptions)
        {
            var asset = _mediaContext.Assets.Create(assetName, assetCreationOptions);

            Console.WriteLine("Asset name: " + asset.Name);
            Console.WriteLine("Time created: " + asset.Created.Date.ToString());

            return asset;
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
            var processor = _mediaContext.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        private static IJob GetJob(string jobId)
        {
            var jobInstance = from j in _mediaContext.Jobs
                              where j.Id == jobId
                              select j;

            IJob job = jobInstance.FirstOrDefault();

            return job;
        }
    }
}
