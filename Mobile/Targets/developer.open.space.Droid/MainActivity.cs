
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.AppIndexing;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using FormsToolkit;
using FormsToolkit.Droid;
using Plugin.Permissions;
using Refractored.XamForms.PullToRefresh.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppLinks;
using Xamarin;
using developer.open.space.Droid.Initializer;
using developer.open.space.Clients.Application;
using Gcm.Client;
using developer.open.space.Utils.Helpers;
using System.IO;

namespace developer.open.space.Droid
{
    [Activity(Label = "Developer Open Space", Icon = "@drawable/newicon", LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Android.Content.Intent.CategoryDefault,
            Android.Content.Intent.CategoryBrowsable
        },
        DataScheme = "http",
        DataPathPrefix = "/session/",
        DataHost = "devopenspace.de")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Android.Content.Intent.CategoryDefault,
            Android.Content.Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataPathPrefix = "/session/",
        DataHost = "devopenspace.de")]

    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Android.Content.Intent.CategoryDefault,
            Android.Content.Intent.CategoryBrowsable
        },
        DataScheme = "http",
        DataHost = "devopenspace.de")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Android.Content.Intent.CategoryDefault,
            Android.Content.Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataHost = "devopenspace.de")]
    public class MainActivity : FormsAppCompatActivity
    {
        private static MainActivity current;
        public static MainActivity Current { get { return current; } }
        GoogleApiClient client;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            FormsMaps.Init(this, savedInstanceState);
            AndroidAppLinks.Init(this);
            Toolkit.Init();
            
            PullToRefreshLayoutRenderer.Init();
            typeof(Color).GetProperty("Accent", BindingFlags.Public | BindingFlags.Static).SetValue(null, Color.FromHex("#757575"));

            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();

            InitializeHockeyApp();

            developer.open.space.Utils.Helpers.Settings.SqlitePath = GetSqlitePlatformPath();

            LoadApplication(new DeveloperOpenSpaceApplication(new DroidInitializer()));

            var gpsAvailable = IsPlayServicesAvailable();
            developer.open.space.Utils.Helpers.Settings.Current.PushNotificationsEnabled = gpsAvailable;

            if (gpsAvailable)
            {
                client = new GoogleApiClient.Builder(this)
                .AddApi(AppIndex.API)
                .Build();
            }

            OnNewIntent(Intent);

            if (!developer.open.space.Utils.Helpers.Settings.Current.PushNotificationsEnabled)
                return;

            RegisterWithGCM();
        }

        private string GetSqlitePlatformPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
        }

        void InitializeHockeyApp()
        {
            if (string.IsNullOrWhiteSpace(ApiKeys.HockeyAppAndroid) || ApiKeys.HockeyAppAndroid == nameof(ApiKeys.HockeyAppAndroid))
                return;
        }

        private void RegisterWithGCM()
        {
            // Check to ensure everything's set up right
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register for push notifications
            System.Diagnostics.Debug.WriteLine("MainActivity", "Registering...");
            GcmService.Initialize(this);
            GcmService.Register(this);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    if (developer.open.space.Utils.Helpers.Settings.Current.GooglePlayChecked)
                        return false;

                    developer.open.space.Utils.Helpers.Settings.Current.GooglePlayChecked = true;
                    Toast.MakeText(this, "Google Play services is not installed, push notifications have been disabled.", ToastLength.Long).Show();
                }
                else
                {
                    developer.open.space.Utils.Helpers.Settings.Current.PushNotificationsEnabled = false;
                }
                return false;
            }
            else
            {
                developer.open.space.Utils.Helpers.Settings.Current.PushNotificationsEnabled = true;
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

