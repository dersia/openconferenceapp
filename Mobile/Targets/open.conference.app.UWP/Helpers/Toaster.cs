using open.conference.app.Clients.ViewModels.Interfaces;
using Windows.UI.Popups;
using Xamarin.Forms;

namespace open.conference.app.UWP
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
