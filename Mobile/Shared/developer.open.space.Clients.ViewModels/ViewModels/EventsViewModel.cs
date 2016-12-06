using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using Prism.Logging;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Xamarin.Forms;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
using developer.open.space.Clients.Views;
using Prism.Services;
using developer.open.space.Utils.Helpers.Extensions;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class EventsViewModel : ViewModelBase, INavigationAware
    {
        public EventsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            Title = "Events";
            if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
            {
                ToolBarItems.Add(new ToolbarItem
                {
                    Text = "Refresh",
                    Icon = "toolbar_refresh.png",
                    Command = ForceRefreshCommand
                });
            }
        }


        public ObservableRangeCollection<FeaturedEvent> Events { get; } = new ObservableRangeCollection<FeaturedEvent>();
        public ObservableRangeCollection<IGrouping<string, FeaturedEvent>> EventsGrouped { get; } = new ObservableRangeCollection<IGrouping<string, FeaturedEvent>>();

        #region Properties
        FeaturedEvent _selectedEvent;
        public FeaturedEvent SelectedEvent
        {
            get { return _selectedEvent; }
            set { SetProperty(ref _selectedEvent, value); }
        }


        #endregion

        #region Sorting


        void SortEvents()
        {
            EventsGrouped.ReplaceRange(Events.GroupByDate());
        }


        #endregion


        #region Commands

        public ICommand EventTappedCommand => new DelegateCommand<ItemTappedEventArgs>((eventArgs) => GoToEvent(eventArgs?.Item as FeaturedEvent));

        private void GoToEvent(FeaturedEvent featuredEvent)
        {
            if (featuredEvent == null)
                return;

            GoToCommand.Execute(new List<string> { nameof(EventDetailsPage) });
            EventAggregator.GetEvent<EventSelectedEvent>().Publish(featuredEvent);
        }

        public ICommand ForceRefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteForceRefreshCommandAsync());

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExecuteLoadEventsAsync(true);
        }

        public ICommand NavigateToEventDetailsCommand => new DelegateCommand<ItemTappedEventArgs>((f) => GoToCommand.Execute(new List<string> { nameof(EventDetailsPage) }));

        public ICommand LoadEventsCommand => DelegateCommand<ItemTappedEventArgs>.FromAsyncHandler(async (f) => await ExecuteLoadEventsAsync());

        async Task<bool> ExecuteLoadEventsAsync(bool force = false)
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;

                Events.ReplaceRange(await StoreManager.EventStore.GetItemsAsync(force));

                Title = "Events (" + Events.Count(e => e.StartTime.HasValue && e.StartTime.Value > DateTime.UtcNow) + ")";

                SortEvents();

            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadEventsAsync", Prism.Logging.Category.Exception, Priority.High);
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
            if (Events.Count == 0)
                LoadEventsCommand.Execute(null);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
