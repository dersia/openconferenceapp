using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
using developer.open.space.Utils.Helpers.Extensions;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class NotificationsViewModel : ViewModelBase, INavigationAware
    {
        public NotificationsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
        }


        public ObservableRangeCollection<Notification> Notifications { get; } = new ObservableRangeCollection<Notification>();
        public ObservableRangeCollection<IGrouping<string, Notification>> NotificationsGrouped { get; } = new ObservableRangeCollection<IGrouping<string, Notification>>();

        void SortNotifications()
        {

            var groups = from notification in Notifications
                         orderby notification.Date descending
                         group notification by notification.Date.GetSortName()
                into notificationGroup
                         select new Grouping<string, Notification>(notificationGroup.Key, notificationGroup);

            NotificationsGrouped.ReplaceRange(groups);
        }

        private Notification _selectedNotification;
        public Notification SelectedNotification
        {
            get { return _selectedNotification; }
            set { SetProperty(ref _selectedNotification, value); }
        }

        public ICommand IgnoreTappingCommand => new DelegateCommand<ItemTappedEventArgs>((eventArgs) => SelectedNotification = null);
        public ICommand ForceRefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteForceRefreshCommandAsync());

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExecuteLoadNotificationsAsync(true);
        }
        
        public ICommand LoadNotificationsCommand => DelegateCommand<bool>.FromAsyncHandler(async (f) => await ExecuteLoadNotificationsAsync());

        async Task<bool> ExecuteLoadNotificationsAsync(bool force = false)
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;

                Notifications.ReplaceRange(await StoreManager.NotificationStore.GetItemsAsync(force));

                SortNotifications();

            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadNotificationsAsync", Prism.Logging.Category.Exception, Priority.High);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            LoadNotificationsCommand.Execute(false);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
