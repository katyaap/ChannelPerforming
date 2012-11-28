namespace ChannelPerforming.Common
{
    public class Utils
    {
        // Configuration Keys
        public const string ConfigurationString = "DataConnectionString";
        public const string CloudQueueKey = "videoqueue";
        public const string CloudBlobKey = "videoblob";

        // In Worker
        public const string MediaServiceAccoutName = "MediaServiceAccoutName";
        public const string MediaServiceAccoutKey = "MediaServiceAccoutKey";

        // Media Progress State Type
        public const string MediaProgressStateTypeWait = "Wait";
        public const string MediaProgressStateTypeFail = "Fail";
        public const string MediaProgressStateTypeBegin = "Begin";
        public const string MediaProgressStateTypeComplete = "Complete";

        // Media Temp Folder
        public const string MediaDownloadFilePath = "\\Assets\\";
    }
}
