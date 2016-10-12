using developer.open.space.Clients.ViewModels.Interfaces;
using Windows.UI.Popups;
using Xamarin.Forms;

namespace developer.open.space.UWP
{
    public class Toaster : IToast
    {
        public void SendToast(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dialog = new MessageDialog(message);
                dialog.ShowAsync();
            });
        }
    }
}
