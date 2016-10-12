using Xamarin.Forms;
using developer.open.space.iOS;
using ToastIOS;
using UIKit;
using CoreGraphics;
using developer.open.space.Clients.ViewModels.Interfaces;

namespace developer.open.space.iOS
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
