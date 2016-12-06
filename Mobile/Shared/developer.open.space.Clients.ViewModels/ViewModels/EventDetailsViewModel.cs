using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Clients.Views;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
using developer.open.space.Utils.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class EventDetailsViewModel : ViewModelBase, INavigationAware
    {
        private FeaturedEvent _selectedEvent;
        public FeaturedEvent SelectedEvent
        {
            get { return _selectedEvent; }
            set { SetProperty(ref _selectedEvent, value); }
        }

        public ObservableRangeCollection<Sponsor> Sponsors { get; set; }

        public EventDetailsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            Sponsors = new ObservableRangeCollection<Sponsor>();
            EventAggregator.GetEvent<EventSelectedEvent>().Subscribe(EventSelected);
        }

        ~EventDetailsViewModel()
        {
            EventAggregator.GetEvent<EventSelectedEvent>().Unsubscribe(EventSelected);
        }

        bool isReminderSet;
        public bool IsReminderSet
        {
            get { return isReminderSet; }
            set { SetProperty(ref isReminderSet, value); }
        }

        private int _listViewHeightAdjustment;
        public int ListViewHeightAdjustment
        {
            get { return _listViewHeightAdjustment; }
            set { SetProperty(ref _listViewHeightAdjustment, value); }
        }

        public ICommand LoadEventDetailsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadEventDetailsCommandAsync());

        private async Task ExecuteLoadEventDetailsCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsReminderSet = await ReminderService.HasReminderAsync("event_" + SelectedEvent.Id);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadEventDetailsCommandAsync", Prism.Logging.Category.Exception, Priority.High);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ICommand ReminderCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteReminderCommandAsync());
        public ICommand NavigateToSponsorDetails => new DelegateCommand(() => GoToCommand.Execute(new List<string> { nameof(SponsorDetailsPage) }));


        private async Task ExecuteReminderCommandAsync()
        {
            if (!IsReminderSet)
            {
				try
                {
					var result = await ReminderService.AddReminderAsync("event_" + SelectedEvent.Id,
						new Plugin.Calendars.Abstractions.CalendarEvent
						{
							Description = SelectedEvent.Description,
							Location = SelectedEvent.LocationName,
							AllDay = SelectedEvent.IsAllDay,
							Name = SelectedEvent.Title,
							Start = SelectedEvent.StartTime.Value,
							End = SelectedEvent.EndTime.Value
						});


					if (!result)
						return;

					Logger.Log($"{DevopenspaceLoggerKeys.ReminderAdded}, Title, {SelectedEvent.Title}", Prism.Logging.Category.Info, Priority.None);
					IsReminderSet = true;
				}
                catch (Exception ex)
                {
                    Logger.Log($"{DevopenspaceLoggerKeys.ReminderAdded}, Title, {SelectedEvent.Title}. {ex}", Prism.Logging.Category.Info, Priority.None);
                    return;
                }
            }
            else
            {
                var result = await ReminderService.RemoveReminderAsync("event_" + SelectedEvent.Id);
                if (!result)
                    return;
                Logger.Log($"{DevopenspaceLoggerKeys.ReminderRemoved}, Title, {SelectedEvent.Title}", Prism.Logging.Category.Info, Priority.None);
                IsReminderSet = false;
            }

        }

        Sponsor _selectedSponsor;
        public Sponsor SelectedSponsor
        {
            get { return _selectedSponsor; }
            set { SetProperty(ref _selectedSponsor, value); }
        }

        public ICommand SponsorTappedCommand => new DelegateCommand<ItemTappedEventArgs>((eventArgs) => GoToSposnorDetails(eventArgs?.Item as Sponsor));

        private void GoToSposnorDetails(Sponsor sponsor)
        {
            if (sponsor == null)
                return;

            GoToCommand.Execute(new List<string> { nameof(SponsorDetailsPage) });
            EventAggregator.GetEvent<SponsorSelectedEvent>().Publish(sponsor);
        }

        private void EventSelected(FeaturedEvent selectedEvent)
        {
            SelectedEvent = selectedEvent;
            Sponsors.Add(SelectedEvent.Sponsor);
            if (Sponsors != null)
                ListViewHeightAdjustment = Sponsors.Count;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            LoadEventDetailsCommand.Execute(null);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
