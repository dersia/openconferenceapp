using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;

namespace developer.open.space.Clients.ViewModels
{
    public class CategoryCellViewViewModel : ViewModelBase
    {
        public CategoryCellViewViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
        }

        private developer.open.space.DataStore.Abstractions.DataObjects.Category _category;
        public developer.open.space.DataStore.Abstractions.DataObjects.Category Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
    }
}
