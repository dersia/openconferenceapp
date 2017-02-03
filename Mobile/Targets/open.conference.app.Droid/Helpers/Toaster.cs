using Xamarin.Forms;
using Plugin.CurrentActivity;
using Android.Widget;
using open.conference.app.Clients.ViewModels.Interfaces;

namespace open.conference.app.Droid
{
    public class Toaster : IToast
    {
        public void SendToast(string message)
        {
            var context = CrossCurrentActivity.Current.Activity ?? Android.App.Application.Context;  
            Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(context, message, ToastLength.Long).Show();
                });

        }
    }
}

