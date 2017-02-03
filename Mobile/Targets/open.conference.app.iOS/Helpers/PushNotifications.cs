
using UIKit;
using System.Threading.Tasks;
using open.conference.app.iOS;
using Xamarin.Forms;
using Foundation;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Utils.Helpers;

namespace open.conference.app.iOS
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

