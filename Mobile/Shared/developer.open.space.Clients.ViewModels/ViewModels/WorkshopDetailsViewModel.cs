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
    public class WorkshopDetailsViewModel : ViewModelBase, INavigationAware
    {
        private Workshop _selectedWorkshop;
        public Workshop SelectedWorkshop
        {
            get { return _selectedWorkshop; }
            set { SetProperty(ref _selectedWorkshop, value); }
        }

        public WorkshopDetailsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            EventAggregator.GetEvent<WorkshopSelectedEvent>().Subscribe(WorkshopSelected);
        }
        ~WorkshopDetailsViewModel()
        {
            EventAggregator.GetEvent<WorkshopSelectedEvent>().Unsubscribe(WorkshopSelected);
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

        public ICommand GoToFeedbackCommand => new DelegateCommand(GoToFeedback);

        private void GoToFeedback()
        {
            GoToCommand.Execute(nameof(FeedbackPage));
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
            await FavoriteService.ToggleFavorite(SelectedWorkshop);
        }

        public ICommand ReminderCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteReminderCommandAsync());

        async Task ExecuteReminderCommandAsync()
        {
            if (!IsReminderSet)
            {
                var result = await ReminderService.AddReminderAsync(SelectedWorkshop.Id,
                    new Plugin.Calendars.Abstractions.CalendarEvent
                    {
                        AllDay = false,
                        Description = SelectedWorkshop.Abstract,
                        Location = SelectedWorkshop.Room?.Name ?? string.Empty,
                        Name = SelectedWorkshop.Title,
                        Start = SelectedWorkshop.StartTime.Value,
                        End = SelectedWorkshop.EndTime.Value
                    });

                if (!result)
                    return;

                Logger.Log($"{DevopenspaceLoggerKeys.ReminderAdded}, Title, {SelectedWorkshop.Title}", Prism.Logging.Category.Info, Priority.None);
                IsReminderSet = true;
            }
            else
            {
                var result = await ReminderService.RemoveReminderAsync(SelectedWorkshop.Id);
                if (!result)
                    return;
                Logger.Log($"{DevopenspaceLoggerKeys.ReminderRemoved}, Title, {SelectedWorkshop.Title}", Prism.Logging.Category.Info, Priority.None);
                IsReminderSet = false;
            }
        }

        public ICommand ShareCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteShareCommandAsync());

        async Task ExecuteShareCommandAsync()
        {
            Logger.Log($"{DevopenspaceLoggerKeys.Share}, Title, {SelectedWorkshop.Title}", Prism.Logging.Category.Info, Priority.None);
            await CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage() { Text = $"Can't wait for \"{SelectedWorkshop.Title}\" at #devspace !", Title = "Share" });
        }

        public ICommand LoadWorkshoCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadWorkshoCommandAsync());

        public async Task ExecuteLoadWorkshoCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {

                IsBusy = true;

                IsReminderSet = await ReminderService.HasReminderAsync(SelectedWorkshop.Id);
                SelectedWorkshop.FeedbackLeft = await StoreManager.FeedbackStore.LeftFeedback(SelectedWorkshop);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadWorkshopCommandAsync", Prism.Logging.Category.Info, Priority.None);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void WorkshopSelected(Workshop selectedWorkshop)
        {
            SelectedWorkshop = selectedWorkshop;
            if (SelectedWorkshop.StartTime.HasValue)
                ShowReminder = !SelectedWorkshop.StartTime.Value.IsTBA();
            else
                ShowReminder = false;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
