using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels
{
    public class WorkshopCellViewViewModel : ViewModelBase
    {
        private FontSizeConverter _fontSizeConverter = new FontSizeConverter();
        public WorkshopCellViewViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
        }

        private Workshop _selectedWorkshop;
        public Workshop SelectedWorkshop
        {
            get { return _selectedWorkshop; }
            set { SetProperty(ref _selectedWorkshop, value); }
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
