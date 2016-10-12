using developer.open.space.Clients.Application;
using developer.open.space.UWP.Initializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace developer.open.space.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            developer.open.space.Utils.Helpers.Settings.SqlitePath = GetSqlitePlatformPath();
            LoadApplication(new DeveloperOpenSpaceApplication(new UwpInitializer(), AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop"));
            ApplicationView.PreferredLaunchViewSize = new Size(1024, 768);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1024, 768));
        }

        private string GetSqlitePlatformPath()
        {
            return ApplicationData.Current.LocalFolder.Path; // Documents folder
        }
    }
}
