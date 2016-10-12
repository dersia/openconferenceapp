using System;
using Foundation;
using UIKit;
using developer.open.space.Clients.ViewModels.Interfaces;
using System.Threading.Tasks;

namespace developer.open.space.iOS
{
    public class LaunchTwitter : ILaunchTwitter
    {
        #region ILaunchTwitter implementation

        public async Task<bool> OpenUserName(string username)
        {
            try
            {
                if(UIApplication.SharedApplication.OpenUrl(NSUrl.FromString($"twitter://user?screen_name={username}")))
                    return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch url" + ex);
            }

            try
            {
                if(UIApplication.SharedApplication.OpenUrl(NSUrl.FromString($"tweetbot://{username}/timeline")))
                    return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch url " + ex);
            }
            return false;
        }

        public async Task<bool> OpenStatus(string statusId)
        {
            
            try
            {
                if(UIApplication.SharedApplication.OpenUrl(NSUrl.FromString($"twitter://status?id={statusId}")))
                    return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch url " + ex);
            }

            try
            {
                if(UIApplication.SharedApplication.OpenUrl(NSUrl.FromString($"tweetbot:///status/{statusId}")))
                    return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to launch url " + ex);
            }
            return false;
        }

        #endregion


    }
}

