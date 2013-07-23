namespace ChannelPerforming.ProtectionPlayer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows.Media;

    public class ManualLicenseAcquirer : LicenseAcquirer
    {
        private string challengeString;
        string _mediaElementName;

        public ManualLicenseAcquirer(string mediaElementName)
        {
            _mediaElementName = mediaElementName;
        }

        protected override void OnAcquireLicense(Stream licenseChallenge, Uri licenseServerUri)
        {
            StreamReader sr = new StreamReader(licenseChallenge);
            challengeString = sr.ReadToEnd();

            Uri resolvedLicenseServerUri;
            if (LicenseServerUriOverride == null)
            {
                resolvedLicenseServerUri = licenseServerUri;
            }
            else
            {
                resolvedLicenseServerUri = LicenseServerUriOverride;
            }

            HttpWebRequest request = WebRequest.Create(resolvedLicenseServerUri) as HttpWebRequest;
            request.Method = "POST";

            request.ContentType = "application/xml";

            request.Headers["msprdrm_server_redirect_compat"] = "false";
            request.Headers["msprdrm_server_exception_compat"] = "false";

            IAsyncResult asyncResult = request.BeginGetRequestStream(new AsyncCallback(RequestStreamCallback), request);
        }

        public void RequestStreamCallback(IAsyncResult ar)
        {
            HttpWebRequest request = ar.AsyncState as HttpWebRequest;

            request.ContentType = "text/xml";
            Stream requestStream = request.EndGetRequestStream(ar);
            StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.Encoding.UTF8);

            streamWriter.Write(challengeString);
            streamWriter.Close();

            request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = ar.AsyncState as HttpWebRequest;
            WebResponse response = request.EndGetResponse(ar);
            SetLicenseResponse(response.GetResponseStream());
        }
    }
}