using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
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
    public class SpeakerDetailsViewModel : ViewModelBase, INavigationAware
    {
        private string _sessionId;
        private string _workshopId;

        #region Propeties

        private double _mainHeight;
        public double MainHeight
        {
            get { return _mainHeight; }
            set { SetProperty(ref _mainHeight, value); }
        }

        private double _speakerTitleHeight;
        public double SpeakerTitleHeight
        {
            get { return _speakerTitleHeight; }
            set { SetProperty(ref _speakerTitleHeight, value); }
        }

        public int MyProperty { get; set; }
        private async Task HandleScrolling(ScrolledEventArgs e)
        {
            if (e.ScrollY > (MainHeight - SpeakerTitleHeight))
                Title = SelectedSpeaker.FirstName;
            else
                Title = "Speaker Info";
        }

        #endregion

        private Speaker _selectedSpeaker;
        public Speaker SelectedSpeaker
        {
            get { return _selectedSpeaker; }
            set { SetProperty(ref _selectedSpeaker, value); }
        }
        public ObservableRangeCollection<Session> Sessions { get; } = new ObservableRangeCollection<Session>();
        public ObservableRangeCollection<Workshop> Workshops { get; } = new ObservableRangeCollection<Workshop>();
        public ObservableRangeCollection<Models.MenuItem> FollowItems { get; } = new ObservableRangeCollection<Models.MenuItem>();

        bool hasAdditionalSessions;
        public bool HasAdditionalSessions
        {
            get { return hasAdditionalSessions; }
            set { SetProperty(ref hasAdditionalSessions, value); }
        }

        bool hasAdditionalWorkshops;
        public bool HasAdditionalWorkshops
        {
            get { return hasAdditionalWorkshops; }
            set { SetProperty(ref hasAdditionalWorkshops, value); }
        }

        public SpeakerDetailsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            EventAggregator.GetEvent<SpeakerSelectedEvent>().Subscribe(SpeakerSelected);
            EventAggregator.GetEvent<SessionSelectedEvent>().Subscribe(SessionSelected);
            EventAggregator.GetEvent<WorkshopSelectedEvent>().Subscribe(WorkshopSelected);
        }

        ~SpeakerDetailsViewModel()
        {
            EventAggregator.GetEvent<SpeakerSelectedEvent>().Unsubscribe(SpeakerSelected);
            EventAggregator.GetEvent<SessionSelectedEvent>().Unsubscribe(SessionSelected);
            EventAggregator.GetEvent<WorkshopSelectedEvent>().Unsubscribe(WorkshopSelected);
        }

        public ICommand LoadSessionsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadSessionsCommandAsync());
        public ICommand LoadWorkshopsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadWorkshopsCommandAsync());

        public async Task ExecuteLoadSessionsCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var items = (await StoreManager.SessionStore.GetSpeakerSessionsAsync(SelectedSpeaker.Id)).Where(x => x.Id != _sessionId);

                Sessions.ReplaceRange(items);

                HasAdditionalSessions = Sessions.Count > 0;
            }
            catch (Exception ex)
            {
                HasAdditionalSessions = false;
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteLoadWorkshopsCommandAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var items = (await StoreManager.WorkshopStore.GetSpeakerWorkshopsAsync(SelectedSpeaker.Id)).Where(x => x.Id != _workshopId);

                Workshops.ReplaceRange(items);

                HasAdditionalWorkshops = Workshops.Count > 0;
            }
            catch (Exception ex)
            {
                HasAdditionalWorkshops = false;
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
            }
            finally
            {
                IsBusy = false;
            }
        }

        Models.MenuItem _selectedFollowItem;
        public Models.MenuItem SelectedFollowItem
        {
            get { return _selectedFollowItem; }
            set
            {
                SetProperty(ref _selectedFollowItem, value);
                OnPropertyChanged();
                if (_selectedFollowItem == null)
                    return;

                LaunchBrowserCommand.Execute(_selectedFollowItem.Parameter);
            }
        }

        Session selectedSession;
        public Session SelectedSession
        {
            get { return selectedSession; }
            set
            {
                selectedSession = value;
                OnPropertyChanged();
                if (selectedSession == null)
                    return;

                EventAggregator.GetEvent<NavigateToSessionEvent>().Publish(selectedSession);

                SelectedSession = null;
            }
        }
        Workshop selectedWorkshop;
        public Workshop SelectedWorkshop
        {
            get { return selectedWorkshop; }
            set
            {
                selectedWorkshop = value;
                OnPropertyChanged();
                if (selectedWorkshop == null)
                    return;

                EventAggregator.GetEvent<NavigateToWorkshopEvent>().Publish(selectedWorkshop);

                SelectedWorkshop = null;
            }
        }
        public ICommand FavoriteCommand => DelegateCommand<Session>.FromAsyncHandler(ExecuteFavoriteCommand);

        public async Task ExecuteFavoriteCommand(Session session)
        {
            var response = await PageDialogService.DisplayAlertAsync("Unfavorite Session",
                "Are you sure you want to remove this session from your favorites?",
                "Cancel",
                "Unfavorite");

            if (response)
            {
                var toggled = await FavoriteService.ToggleFavorite(session);
                if (toggled)
                    await ExecuteLoadSessionsCommandAsync();
            }
        }

        public ICommand FavoriteWorkshopCommand => DelegateCommand<Workshop>.FromAsyncHandler(ExecuteFavoriteWorkshopCommand);

        public async Task ExecuteFavoriteWorkshopCommand(Workshop workshop)
        {
            var response = await PageDialogService.DisplayAlertAsync("Unfavorite workshop",
                "Are you sure you want to remove this workshop from your favorites?",
                "Cancel",
                "Unfavorite");

            if (response)
            {
                var toggled = await FavoriteService.ToggleFavorite(workshop);
                if (toggled)
                    await ExecuteLoadWorkshopsCommandAsync();
            }
        }

        private void SpeakerSelected(Speaker speaker)
        {
            SelectedSpeaker = speaker;
            if (!string.IsNullOrWhiteSpace(speaker.CompanyWebsiteUrl))
            {
                FollowItems.Add(new Models.MenuItem
                {
                    Name = "Web",
                    Subtitle = speaker.CompanyWebsiteUrl,
                    Parameter = speaker.CompanyWebsiteUrl,
                    Icon = "icon_website.png"
                });
            }

            if (!string.IsNullOrWhiteSpace(speaker.BlogUrl))
            {
                FollowItems.Add(new Models.MenuItem
                {
                    Name = "Blog",
                    Subtitle = speaker.BlogUrl,
                    Parameter = speaker.BlogUrl,
                    Icon = "icon_blog.png"
                });
            }

            if (!string.IsNullOrWhiteSpace(speaker.TwitterUrl))
            {
                FollowItems.Add(new Models.MenuItem
                {
                    Name = Device.OS == TargetPlatform.iOS ? "Twitter" : speaker.TwitterUrl,
                    Subtitle = $"@{speaker.TwitterUrl}",
                    Parameter = "http://twitter.com/" + speaker.TwitterUrl,
                    Icon = "icon_twitter.png"
                });
            }

            if (!string.IsNullOrWhiteSpace(speaker.LinkedInUrl))
            {
                FollowItems.Add(new Models.MenuItem
                {
                    Name = "Social",
                    Subtitle = speaker.LinkedInUrl,
                    Parameter = speaker.LinkedInUrl,
                    Icon = "icon_linkedin.png"
                });
            }
        }
        private void SessionSelected(Session selectedSession)
        {
            _sessionId = selectedSession?.Id;
        }

        private void WorkshopSelected(Workshop selectedWorkshop)
        {
            _workshopId = selectedWorkshop?.Id;
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

        #region Commands

        private ICommand MainScrollCommand => DelegateCommand<ScrolledEventArgs>.FromAsyncHandler(async (e) => await HandleScrolling(e));

        public ICommand IgnoreTapCommand => new DelegateCommand<ItemTappedEventArgs>((args) => SelectedFollowItem = null);

        #endregion

    }
}
