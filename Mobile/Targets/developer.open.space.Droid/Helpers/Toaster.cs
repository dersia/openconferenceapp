using Xamarin.Forms;
using Plugin.CurrentActivity;
using Android.Widget;
using developer.open.space.Clients.ViewModels.Interfaces;

namespace developer.open.space.Droid
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

