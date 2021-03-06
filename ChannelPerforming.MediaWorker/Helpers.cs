﻿namespace ChannelPerforming.MediaWorker
{
    using System.IO;
    using System.Net;

    internal class Helpers
    {
        public static string DownloadAssetToLocal(string url, string dowlandfolder)
        {
            WebClient client = new WebClient();

            string filepath = Path.GetFileName(url);
            string dowlandfilePath = string.Format("{0}\\{1}", dowlandfolder, filepath);
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            client.DownloadFile(url, dowlandfilePath);

            return dowlandfilePath;
        }
    }
}
