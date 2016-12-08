using Xamarin.Forms;
using open.conference.app.iOS;
using ToastIOS;
using UIKit;
using CoreGraphics;
using open.conference.app.Clients.ViewModels.Interfaces;

namespace open.conference.app.iOS
{
    public class Toaster : IToast
    {
        public void SendToast(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(message, Toast.LENGTH_LONG).SetCornerRadius(0).Show();
                });
        }
    }
}
