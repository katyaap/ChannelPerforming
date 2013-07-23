namespace ChannelPerforming.ProtectionPlayer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class MainPage : UserControl
    {
        private string LicenseServer = "https://play-lic.cimcontent.net/playready/RightsManager.asmx";
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        public void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Player.LicenseAcquirer = new ManualLicenseAcquirer(Player.Name);
            Player.LicenseAcquirer.LicenseServerUriOverride = new Uri(LicenseServer, UriKind.Absolute);

            Player.Source = ((App)Application.Current).MediaUrl;
            Player.MediaFailed += Player_MediaFailed;
            
            Player.LicenseAcquirer.AcquireLicenseCompleted += LicenseAcquirer_AcquireLicenseCompleted;
        }

        public void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Alert(e.ErrorException.Message);
        }

        public void LicenseAcquirer_AcquireLicenseCompleted(object sender, AcquireLicenseCompletedEventArgs e)
        {
            Player.Play();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Player.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
        }
    }
}
