using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels.ViewModels
{
    public class SimpleNavigationViewModel : ViewModelBase
    {
        public Color BarBackgroundColor
        {
            get
            {
                return (Color)Application.Current.Resources["DarkGrey"];
            }
        }

        public Color BarTextColor
        {
            get { return (Color)Application.Current.Resources["White"]; }
        }

        public SimpleNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {

        }
    }
}
