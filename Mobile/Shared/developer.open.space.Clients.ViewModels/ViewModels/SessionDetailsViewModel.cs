using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Utils.Helpers.Extensions;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
using developer.open.space.Utils.Helpers;
using Plugin.Share;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using developer.open.space.Clients.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class SessionDetailsViewModel : ViewModelBase, INavigationAware
    {
        private Session _selectedSession;
        public Session SelectedSession
        {
            get { return _selectedSession; }
            set { SetProperty(ref _selectedSession, value); }
        }

        public SessionDetailsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            EventAggregator.GetEvent<SessionSelectedEvent>().Subscribe(SessionSelected);
        }

        ~SessionDetailsViewModel()
        {
            EventAggregator.GetEvent<SessionSelectedEvent>().Unsubscribe(SessionSelected);
        }

        public bool ShowReminder { get; set; }

        private bool _isReminderSet;
        public bool IsReminderSet
        {
            get { return _isReminderSet; }
            set { SetProperty(ref _isReminderSet, value); }
        }

        private Speaker _selectedSpeaker;
        public Speaker SelectedSpeaker
        {
            get { return _selectedSpeaker; }
            set { SetProperty(ref _selectedSpeaker, value); }
        }

        public ICommand GoToSpeakerCommand => new DelegateCommand<ItemTappedEventArgs>((eventArgs) => GoToSpeaker(eventArgs?.Item as Speaker));

        private void GoToSpeaker(Speaker selectedSpeaker)
        {
            if (selectedSpeaker == null)
                return;

            GoToCommand.Execute(new List<string> { nameof(SpeakerDetailsPage) });
            EventAggregator.GetEvent<SpeakerSelectedEvent>().Publish(selectedSpeaker);
        }

        public ICommand FavoriteCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteFavoriteCommandAsync());

        private async Task ExecuteFavoriteCommandAsync()
        {
            await FavoriteService.ToggleFavorite(SelectedSession);
        }

        public ICommand ReminderCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteReminderCommandAsync());

        async Task ExecuteReminderCommandAsync()
        {
            if (!IsReminderSet)
            {
                var result = await ReminderService.AddReminderAsync(SelectedSession.Id,
                    new Plugin.Calendars.Abstractions.CalendarEvent
                    {
                        AllDay = false,
                        Description = SelectedSession.Abstract,
                        Location = SelectedSession.Room?.Name ?? string.Empty,
                        Name = SelectedSession.Title,
                        Start = SelectedSession.StartTime.Value,
                        End = SelectedSession.EndTime.Value
                    });

                if (!result)
                    return;

                Logger.Log($"{DevopenspaceLoggerKeys.ReminderAdded}, Title, {SelectedSession.Title}", Prism.Logging.Category.Info, Priority.None);
                IsReminderSet = true;
            }
            else
            {
                var result = await ReminderService.RemoveReminderAsync(SelectedSession.Id);
                if (!result)
                    return;
                Logger.Log($"{DevopenspaceLoggerKeys.ReminderRemoved}, Title, {SelectedSession.Title}", Prism.Logging.Category.Info, Priority.None);
                IsReminderSet = false;
            }
        }

        public ICommand ShareCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteShareCommandAsync());

        async Task ExecuteShareCommandAsync()
        {
            Logger.Log($"{DevopenspaceLoggerKeys.Share}, Title, {SelectedSession.Title}", Prism.Logging.Category.Info, Priority.None);
            await CrossShare.Current.Share($"Can't wait for \"{SelectedSession.Title}\" at #devspace !", "Share");
        }

        public ICommand LoadSessionCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadSessionCommandAsync());

        public async Task ExecuteLoadSessionCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {

                IsBusy = true;

                IsReminderSet = await ReminderService.HasReminderAsync(SelectedSession.Id);
                SelectedSession.FeedbackLeft = await StoreManager.FeedbackStore.LeftFeedback(SelectedSession);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadSessionCommandAsync", Prism.Logging.Category.Info, Priority.None);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SessionSelected(Session selectedSession)
        {
            SelectedSession = selectedSession;
            if (SelectedSession.StartTime.HasValue)
                ShowReminder = !SelectedSession.StartTime.Value.IsTBA();
            else
                ShowReminder = false;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
