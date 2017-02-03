using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels
{
    public class SessionCellViewViewModel : ViewModelBase
    {
        private FontSizeConverter _fontSizeConverter = new FontSizeConverter();
        public SessionCellViewViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
        }

        private Session _selectedSession;
        public Session SelectedSession
        {
            get { return _selectedSession; }
            set { SetProperty(ref _selectedSession, value); }
        } 

        public ICommand FavoriteCommand { get; set; }

        public bool ImageCircleVisible
        {
            get { return Device.OS == TargetPlatform.iOS; }
        }
        public bool BoxViewVisible
        {
            get { return Device.OS != TargetPlatform.iOS; }
        }

        public int FontSize
        {
            get { return Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone ? 10 : 12; }
        }
    }
}
