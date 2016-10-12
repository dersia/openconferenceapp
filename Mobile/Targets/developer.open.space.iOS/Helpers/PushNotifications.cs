
using UIKit;
using System.Threading.Tasks;
using developer.open.space.iOS;
using Xamarin.Forms;
using Foundation;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Utils.Helpers;

namespace developer.open.space.iOS
{
    public class PushNotifications : IPushNotifications
    {
        #region IPushNotifications implementation

        public Task<bool> RegisterForNotifications()
        {
            Settings.Current.PushNotificationsEnabled = true;
            Settings.Current.AttemptedPush = true;

            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes (
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                new NSSet ());

            UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications ();
            
            return Task.FromResult(true);
        }
        public bool IsRegistered
        {
            get
            {
                return UIApplication.SharedApplication.IsRegisteredForRemoteNotifications &&
                    UIApplication.SharedApplication.CurrentUserNotificationSettings.Types != UIUserNotificationType.None;
            }
        }


        public void OpenSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }
        #endregion
    }
}

