using ChannelPerforming.Common;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using TaskCreationOptions = Microsoft.WindowsAzure.MediaServices.Client.TaskCreationOptions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace BilgeAdam.ChannelShow.MediaWorker
{
    internal class WrappedMedia
    {
        private static CloudMediaContext mediaContext;

        static WrappedMedia()
        {
            string accoutName = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutName);
            string accoutKey = RoleEnvironment.GetConfigurationSettingValue(Utils.MediaServiceAccoutKey);

            mediaContext = new CloudMediaContext(accoutName, accoutKey);
        }

        public static string CreateEncodingJob(string inputMediaFilePath)
        {
            IAsset asset = mediaContext.Assets.Create(inputMediaFilePath,
                AssetCreationOptions.StorageEncrypted);

            IJob job = mediaContext.Jobs.Create("My encoding job");

            IMediaProcessor processor = GetMediaProcessor("Windows Azure Media Encoder");

            string configuration = File.ReadAllText(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "MP4 to Smooth Streams.xml"));
            ITask task = job.Tasks.AddNew("My encoding task",
                processor,
                configuration,
                TaskCreationOptions.ProtectedConfiguration);

            task.InputMediaAssets.Add(asset);
            task.OutputMediaAssets.AddNew("Output asset",
                true,
                AssetCreationOptions.None);

            // Launch the job. 
            job.Submit();
            asset.Publish();
            // Checks job progress and prints to the console. 

            job = GetJob(job.Id);

            IAsset outputAsset = job.OutputMediaAssets[0];

            IAccessPolicy thePolicy =
                mediaContext.AccessPolicies.Create("My one-hour readonly policy",
                    TimeSpan.FromHours(1),
                    AccessPermissions.Read);
            ILocator locator = mediaContext.Locators.CreateSasLocator(outputAsset,
                thePolicy,
                DateTime.UtcNow.AddMinutes(-5));

            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);
            if (sasUrlList.Count == 0)
            {
                return string.Empty;
            }

            return sasUrlList[0];
        }

        // Get a media processor reference.
        private static IMediaProcessor GetMediaProcessor(string mediaProcessor)
        {
            // Query for a media processor to get a reference.
            var theProcessor = from p in mediaContext.MediaProcessors
                               where p.Name == mediaProcessor
                               select p;

            // Cast the reference to an IMediaprocessor.
            IMediaProcessor processor = theProcessor.First();

            if (processor == null)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown processor", mediaProcessor));
            }

            return processor;
        }

        public static string CreateThumbnailTask(string inputFilePath)
        {
            // Read the preset configuration data into a string. 
            string xmlPreset = File.ReadAllText(Path.GetFullPath(string.Format("{0}\\approot\\{1}", Environment.GetEnvironmentVariable("rdRoleRoot"), "Thumbnail.xml")));

            IAsset asset = mediaContext.Assets.Create(inputFilePath);

            // Declare a new job.
            IJob job = mediaContext.Jobs.Create("My Thumbnail job");
            IMediaProcessor processor = GetMediaProcessor("Windows Azure Media Encoder");

            // Create a task with the encoding details, using a configuration file with presets.
            ITask task = job.Tasks.AddNew("My thumbnail task",
                processor,
                xmlPreset,
                TaskCreationOptions.ProtectedConfiguration);
            // Specify the input asset to be encoded.
            task.InputMediaAssets.Add(asset);
            // Add an output asset to contain the results of the job.
            task.OutputMediaAssets.AddNew("Output asset", true, AssetCreationOptions.None);

            // Launch the job. 
            job.Submit();

            job = GetJob(job.Id);
            IAsset outputAsset = job.OutputMediaAssets[0];

            IAccessPolicy thePolicy =
                mediaContext.AccessPolicies.Create("My one-hour readonly policy",
                    TimeSpan.FromHours(1),
                    AccessPermissions.Read);

            ILocator locator = mediaContext.Locators.CreateSasLocator(outputAsset,
                thePolicy,
                DateTime.UtcNow.AddMinutes(-5));

            // Build a list of SAS URLs to each file in the asset. 
            List<String> sasUrlList = GetAssetSasUrlList(outputAsset, locator);
            if (sasUrlList.Count > 0)
            {
                return sasUrlList[0];
            }

            return string.Empty;
        }

        // Create and return a SAS URL to a single file in an asset. 
        static string BuildFileSasUrl(IFileInfo file, ILocator locator)
        {
            var uriBuilder = new UriBuilder(locator.Path);
            uriBuilder.Path += "/" + file.Name;

            return uriBuilder.Uri.AbsoluteUri;
        }

        // Create a list of SAS URLs to all files in an asset.
        private static List<String> GetAssetSasUrlList(IAsset asset, ILocator locator)
        {
            // Declare a list to contain all the SAS URLs.
            List<String> fileSasUrlList = new List<String>();

            // If the asset has files, build a list of URLs to 
            // each file in the asset and return. 
            foreach (IFileInfo file in asset.Files)
            {
                string sasUrl = BuildFileSasUrl(file, locator);
                fileSasUrlList.Add(sasUrl);
            }

            // Return the list of SAS URLs.
            return fileSasUrlList;
        }

        static IJob GetJob(string jobId)
        {
            // Use a Linq select query to get an updated reference by Id. 
            var job = from j in mediaContext.Jobs
                      where j.Id == jobId
                      select j;

            // Return the job reference as an Ijob. 
            IJob theJob = job.SingleOrDefault();

            // Confirm whether job exists, and return. 
            if (theJob != null)
            {
                return theJob;
            }

            return null;
        }
    }
}
