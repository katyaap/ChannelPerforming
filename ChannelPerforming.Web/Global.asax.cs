using System;
using System.Web;
using System.Web.Optimization;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime; 

namespace ChannelPerforming.Web
{
    public class Global : HttpApplication 
    {
        public void Application_Start(object sender, EventArgs e)
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)));

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterOpenAuth();
        }

        public void Application_End(object sender, EventArgs e)
        {

        }

        public void Application_Error(object sender, EventArgs e)
        {

        }
    }
}
