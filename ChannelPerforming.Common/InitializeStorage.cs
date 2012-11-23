namespace ChannelPerforming.Common
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class InitializeStorage
    {
        private static CloudBlobClient _blobClient;
        private static CloudQueueClient _queueClient;

        private static object _lock = new object();

        public static CloudBlobClient BlobClient
        {
            get { return _blobClient; }
            set { _blobClient = value; }
        }

        public static CloudQueueClient QueueClient
        {
            get { return _queueClient; }
            set { _queueClient = value; }
        }

        public static void Initialize()
        {
            lock (_lock)
            {
                CloudStorageAccount storageAccount =
                    CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);
                storageAccount.CreateCloudTableClient();

                _blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = _blobClient.GetContainerReference(Utils.CloudBlobKey);

                BlobContainerPermissions permissions = container.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
                container.CreateIfNotExist();

                QueueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = QueueClient.GetQueueReference(Utils.CloudQueueKey);
                queue.CreateIfNotExist();
            }
        }
    }
}
