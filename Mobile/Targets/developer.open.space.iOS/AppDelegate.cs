using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using developer.open.space.Clients.Views;
using developer.open.space.Clients.Application;
using developer.open.space.iOS.Initializer;

using Social;
using developer.open.space.DataStore.Abstractions;
using Prism.Services;
using developer.open.space.DataStore.Mock.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using developer.open.space.Clients.ViewModels.Models;
using Xamarin;
using FormsToolkit.iOS;
using developer.open.space.iOS;
using Refractored.XamForms.PullToRefresh.iOS;
using developer.open.space.Utils.Helpers;
using WindowsAzure.Messaging;
using System.IO;

namespace developer.open.space.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {

        public static class ShortcutIdentifier
        {
            public const string Tweet = "de.developer.open.space.tweet";
            public const string Announcements = "de.developer.open.space.announcements";
            public const string Events = "de.developer.open.space.events";
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            var tint = UIColor.FromRGB(207, 80, 55);
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(255, 233, 200); //bar background
            UINavigationBar.Appearance.TintColor = tint; //Tint color of button items

            UIBarButtonItem.Appearance.TintColor = tint; //Tint color of button items

            UITabBar.Appearance.TintColor = tint;

            UISwitch.Appearance.OnTintColor = tint;

            UIAlertView.Appearance.TintColor = tint;

            UIView.AppearanceWhenContainedIn(typeof(UIAlertController)).TintColor = tint;
            UIView.AppearanceWhenContainedIn(typeof(UIActivityViewController)).TintColor = tint;
            UIView.AppearanceWhenContainedIn(typeof(SLComposeViewController)).TintColor = tint;

            Forms.Init();
            FormsMaps.Init();
            Toolkit.Init();

            // Code for starting up the Xamarin Test Cloud Agent

            SetMinimumBackgroundFetchInterval();

            //Random Inits for Linking out.
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            SQLitePCL.CurrentPlatform.Init();
            Plugin.Share.ShareImplementation.ExcludedUIActivityTypes = new List<NSString>
            {
                UIActivityType.PostToFacebook,
                UIActivityType.AssignToContact,
                UIActivityType.OpenInIBooks,
                UIActivityType.PostToVimeo,
                UIActivityType.PostToFlickr,
                UIActivityType.SaveToCameraRoll
            };
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            NonScrollableListViewRenderer.Initialize();
            SelectedTabPageRenderer.Initialize();
            TextViewValue1Renderer.Init();
            PullToRefreshLayoutRenderer.Init();

            developer.open.space.Utils.Helpers.Settings.SqlitePath = GetSqlitePlatformPath();

            LoadApplication(new DeveloperOpenSpaceApplication(new IosInitializer()));




            // Process any potential notification data from launch
            ProcessNotification(options);

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, DidBecomeActive);



            try
            {
                return base.FinishedLaunching(app, options);
            }
            catch (Exception ex)
            {
                var test = "";
                throw;
            }
        }

        private string GetSqlitePlatformPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Documents folder
        }


        void DidBecomeActive(NSNotification notification)
        {
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
        }



        public override void RegisteredForRemoteNotifications(UIApplication app, NSData deviceToken)
        {
            if (ApiKeys.AzureServiceBusUrl == nameof(ApiKeys.AzureServiceBusUrl))
                return;

            // Connection string from your azure dashboard
            var cs = SBConnectionString.CreateListenAccess(
                new NSUrl(ApiKeys.AzureServiceBusUrl),
                ApiKeys.AzureKey);

            // Register our info with Azure
            var hub = new SBNotificationHub(cs, ApiKeys.AzureHubName);
            hub.RegisterNativeAsync(deviceToken, null, err => {
                if (err != null)
                    Console.WriteLine("Error: " + err.Description);
                else
                    Console.WriteLine("Success");
            });
        }

        public override void ReceivedRemoteNotification(UIApplication app, NSDictionary userInfo)
        {
            // Process a notification received while the app was already open
            ProcessNotification(userInfo);
        }

        void ProcessNotification(NSDictionary userInfo)
        {
            if (userInfo == null)
                return;

            Console.WriteLine("Received Notification");

            var apsKey = new NSString("aps");

            if (userInfo.ContainsKey(apsKey))
            {

                var alertKey = new NSString("alert");

                var aps = (NSDictionary)userInfo.ObjectForKey(apsKey);

                if (aps.ContainsKey(alertKey))
                {
                    var alert = (NSString)aps.ObjectForKey(alertKey);

                    try
                    {

                        var avAlert = new UIAlertView("Developer Open Space Update", alert, null, "OK", null);
                        avAlert.Show();

                    }
                    catch (Exception ex)
                    {

                    }

                    Console.WriteLine("Notification: " + alert);
                }
            }
        }

        #region Quick Action

        public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("OnActivated");

            // Handle any shortcut item being selected
            HandleShortcutItem(LaunchedShortcutItem);



            // Clear shortcut after it's been handled
            LaunchedShortcutItem = null;
        }
        // if app is already running
        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            Console.WriteLine("PerformActionForShortcutItem");
            // Perform action
            var handled = HandleShortcutItem(shortcutItem);
            completionHandler(handled);
        }

        public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem)
        {
            Console.WriteLine("HandleShortcutItem ");
            var handled = false;

            // Anything to process?
            if (shortcutItem == null)
                return false;


            // Take action based on the shortcut type
            switch (shortcutItem.Type)
            {
                case ShortcutIdentifier.Tweet:
                    Console.WriteLine("QUICKACTION: Tweet");
                    var slComposer = SLComposeViewController.FromService(SLServiceType.Twitter);
                    if (slComposer == null)
                    {
                        new UIAlertView("Unavailable", "Twitter is not available, please sign in on your devices settings screen.", null, "OK").Show();
                    }
                    else
                    {
                        slComposer.SetInitialText("#Devopenspace");
                        if (slComposer.EditButtonItem != null)
                        {
                            slComposer.EditButtonItem.TintColor = UIColor.FromRGB(255, 233, 200);
                        }
                        slComposer.CompletionHandler += (result) =>
                        {
                            InvokeOnMainThread(() => UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null));
                        };

                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewControllerAsync(slComposer, true);
                    }
                    handled = true;
                    break;
                case ShortcutIdentifier.Announcements:
                    Console.WriteLine("QUICKACTION: Accouncements");
                    ContinueNavigation(AppPage.Notification);
                    handled = true;
                    break;
                case ShortcutIdentifier.Events:
                    Console.WriteLine("QUICKACTION: Events");
                    ContinueNavigation(AppPage.Events);
                    handled = true;
                    break;
            }

            Console.Write(handled);
            // Return results
            return handled;
        }

        void ContinueNavigation(AppPage page, string id = null)
        {
            Console.WriteLine("ContinueNavigation");

            // TODO: display UI in Forms somehow
            System.Console.WriteLine("Show the page for " + page);
        }

        #endregion

        #region Background Refresh

        private void SetMinimumBackgroundFetchInterval()
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(MINIMUM_BACKGROUND_FETCH_INTERVAL);
        }

        // Minimum number of seconds between a background refresh this is shorter than Android because it is easily killed off.
        // 20 minutes = 20 * 60 = 1200 seconds
        private const double MINIMUM_BACKGROUND_FETCH_INTERVAL = 1200;

        // Called whenever your app performs a background fetch
        public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Do Background Fetch
            var downloadSuccessful = false;
            try
            {
                Xamarin.Forms.Forms.Init();//need for dependency services
                // Download data
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "PerformFetch";
            }

            // If you don't call this, your application will be terminated by the OS.
            // Allows OS to collect stats like data cost and power consumption
            if (downloadSuccessful)
            {
                completionHandler(UIBackgroundFetchResult.NewData);
            }
            else
            {
                completionHandler(UIBackgroundFetchResult.Failed);
            }
        }

        #endregion
    }
}
