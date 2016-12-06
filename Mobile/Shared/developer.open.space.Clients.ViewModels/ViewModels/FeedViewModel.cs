using Newtonsoft.Json;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Clients.ViewModels.Models;
using developer.open.space.Clients.Views;
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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using developer.open.space.Clients.ViewModels.Models.Extensions;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class FeedViewModel : ViewModelBase, INavigationAware
    {
        public ObservableRangeCollection<Tweet> Tweets { get; } = new ObservableRangeCollection<Tweet>();
        public ObservableRangeCollection<SessionCellViewViewModel> Sessions { get; } = new ObservableRangeCollection<SessionCellViewViewModel>();
        public ObservableRangeCollection<WorkshopCellViewViewModel> Workshops { get; } = new ObservableRangeCollection<WorkshopCellViewViewModel>();
        public DateTime NextForceRefresh { get; set; }
        private bool _firstLoad = true;
        public FeedViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
            if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
            {
                ToolBarItems.Add(new ToolbarItem
                {
                    Text = "Refresh",
                    Icon = "toolbar_refresh.png",
                    Command = RefreshCommand
                });
            }
            LoggedIn = Settings.Email;

            Tweets.CollectionChanged += (sender, e) =>
            {
                TweetListHeightAdjustment = Tweets.Count;
            };

            Sessions.CollectionChanged += (sender, e) =>
            {
                SessionListHeightAdjustment = Sessions.Count;
            };
            Workshops.CollectionChanged += (sender, e) =>
            {
                WorkshopListHeightAdjustment = Workshops.Count;
            };
        }

        private int _tweetListHeightAdjustment;
        public int TweetListHeightAdjustment
        {
            get { return _tweetListHeightAdjustment; }
            set { SetProperty(ref _tweetListHeightAdjustment, value); }
        }

        private int _sessionListHeightAdjustment;
        public int SessionListHeightAdjustment
        {
            get { return _sessionListHeightAdjustment; }
            set { SetProperty(ref _sessionListHeightAdjustment, value); }
        }

        private int _workshopListHeightAdjustment;
        public int WorkshopListHeightAdjustment
        {
            get { return _workshopListHeightAdjustment; }
            set { SetProperty(ref _workshopListHeightAdjustment, value); }
        }

        private string _loggedIn;
        public string LoggedIn
        {
            get { return _loggedIn; }
            set { SetProperty(ref _loggedIn, value); }
        }

        private DateTime _favoritesTime;
        public DateTime FavoritesTime
        {
            get { return _favoritesTime; }
            set { SetProperty(ref _favoritesTime, value); }
        }

        public ICommand RefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteRefreshCommandAsync());

        async Task ExecuteRefreshCommandAsync()
        {
            try
            {
                NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
                IsBusy = true;
                var tasks = new Task[]
                    {
                        ExecuteLoadNotificationsCommandAsync(),
                        ExecuteLoadSocialCommandAsync(),
                        ExecuteLoadSessionsCommandAsync(),
                        ExecuteLoadWorkshopsCommandAsync()
                    };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteRefreshCommandAsync";
                Logger.Log($"{ex.Message}", Prism.Logging.Category.Exception, Priority.High);
            }
            finally
            {
                IsBusy = false;
            }
        }

        Notification notification;
        public Notification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, value); }
        }

        bool loadingNotifications;
        public bool LoadingNotifications
        {
            get { return loadingNotifications; }
            set { SetProperty(ref loadingNotifications, value); }
        }

        public ICommand LoadNotificationsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadNotificationsCommandAsync());

        async Task ExecuteLoadNotificationsCommandAsync()
        {
            if (LoadingNotifications)
                return;
            LoadingNotifications = true;

            try
            {
                Notification = await StoreManager.NotificationStore.GetLatestNotification();
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteLoadNotificationsCommandAsync";
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                Notification = new Notification
                {
                    Date = DateTime.UtcNow,
                    Text = "Welcome to Developer Open Space 2016!"
                };
            }
            finally
            {
                LoadingNotifications = false;
            }
        }

        bool loadingSessions;
        public bool LoadingSessions
        {
            get { return loadingSessions; }
            set { SetProperty(ref loadingSessions, value); }
        }

        bool loadingWorkshops;
        public bool LoadingWorkshops
        {
            get { return loadingWorkshops; }
            set { SetProperty(ref loadingWorkshops, value); }
        }

        private WorkshopCellViewViewModel WorkshopCellViewModelFactory(Workshop workshop)
        {
            return new WorkshopCellViewViewModel(Navigation, EventAggregator, StoreManager, Toast, FavoriteService, Logger, LaunchTwitter, SSOClient, PushNotifications, ReminderService, PageDialogService)
            {
                SelectedWorkshop = workshop,
                FavoriteCommand = FavoriteCommand
            };
        }

        private SessionCellViewViewModel SessionCellViewModelFactory(Session session)
        {
            return new SessionCellViewViewModel(Navigation, EventAggregator, StoreManager, Toast, FavoriteService, Logger, LaunchTwitter, SSOClient, PushNotifications, ReminderService, PageDialogService)
            {
                SelectedSession = session,
                FavoriteCommand = FavoriteCommand
            };
        }

        public ICommand LoadSessionsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadSessionsCommandAsync());
        public ICommand LoadWorkshopsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadWorkshopsCommandAsync());

        async Task ExecuteLoadWorkshopsCommandAsync()
        {
            if (LoadingWorkshops)
                return;

            LoadingWorkshops = true;

            try
            {
                NoWorkshops = false;
                Workshops.Clear();
                OnPropertyChanged("Workshops");
                var workshops = (await StoreManager.WorkshopStore.GetNextWorkshops()).ToWorkshopCellViews(WorkshopCellViewModelFactory);
                if (workshops != null)
                    Workshops.AddRange(workshops);

                NoWorkshops = Workshops.Count == 0;
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteLoadWorkshopsCommandAsync";
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                NoWorkshops = true;
            }
            finally
            {
                LoadingWorkshops = false;
            }

        }

        async Task ExecuteLoadSessionsCommandAsync()
        {
            if (LoadingSessions)
                return;

            LoadingSessions = true;

            try
            {
                NoSessions = false;
                Sessions.Clear();
                OnPropertyChanged("Sessions");
                var sessions = (await StoreManager.SessionStore.GetNextSessions()).ToSessionCellViews(SessionCellViewModelFactory);
                if (sessions != null)
                    Sessions.AddRange(sessions);

                NoSessions = Sessions.Count == 0;
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteLoadSessionsCommandAsync";
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                NoSessions = true;
            }
            finally
            {
                LoadingSessions = false;
            }

        }

        bool noSessions;
        public bool NoSessions
        {
            get { return noSessions; }
            set { SetProperty(ref noSessions, value); }
        }

        bool noWorkshops;
        public bool NoWorkshops
        {
            get { return noWorkshops; }
            set { SetProperty(ref noWorkshops, value); }
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

        bool loadingSocial;
        public bool LoadingSocial
        {
            get { return loadingSocial; }
            set { SetProperty(ref loadingSocial, value); }
        }


        public ICommand LoadSocialCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteLoadSocialCommandAsync());
        public ICommand NavigateToNotifications => new DelegateCommand(() => GoToCommand.Execute(new List<string> { nameof(NotificationsPage) }));

        private async Task ExecuteLoadSocialCommandAsync()
        {
            if (LoadingSocial)
                return;

            LoadingSocial = true;
            try
            {
                SocialError = false;
                Tweets.Clear();

                using (var client = new HttpClient())
                {
                    await StoreManager.InitializeAsync();

                    if (StoreManager is developer.open.space.DataStore.Azure.StoreManager)
                    {
                        var mobileClient = developer.open.space.DataStore.Azure.StoreManager.MobileService;
                        if (mobileClient == null)
                            return;

                        var json = await mobileClient.InvokeApiAsync<string>("Tweet", HttpMethod.Get, null);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            SocialError = true;
                            return;
                        }

                        Tweets.ReplaceRange(JsonConvert.DeserializeObject<List<Tweet>>(json));
                    }


                }

            }
            catch (Exception ex)
            {
                SocialError = true;
                ex.Data["method"] = "ExecuteLoadSocialCommandAsync";
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
            }
            finally
            {
                LoadingSocial = false;
            }

        }

        bool socialError;
        public bool SocialError
        {
            get { return socialError; }
            set { SetProperty(ref socialError, value); }
        }

        Tweet selectedTweet;
        public Tweet SelectedTweet
        {
            get { return selectedTweet; }
            set
            {
                selectedTweet = value;
                OnPropertyChanged();
                if (selectedTweet == null)
                    return;

                LaunchBrowserCommand.Execute(selectedTweet.Url);

                SelectedTweet = null;
            }
        }

        public ICommand FavoriteCommand => DelegateCommand<SessionCellViewViewModel>.FromAsyncHandler(ExecuteFavoriteCommand);
        public ICommand FavoriteWorkshopCommand => DelegateCommand<WorkshopCellViewViewModel>.FromAsyncHandler(ExecuteFavoriteWorkshopCommand);

        private async Task ExecuteFavoriteCommand(SessionCellViewViewModel session)
        {
            if (session?.SelectedSession == null)
                return;

            var response = await PageDialogService.DisplayAlertAsync("Unfavorite Session",
                "Are you sure you want to remove this session from your favorites?",
                "Unfavorite",
                "Cancel");

            if (response)
            {
                var toggled = await FavoriteService.ToggleFavorite(session.SelectedSession);
                if (toggled)
                {
                    await ExecuteLoadSessionsCommandAsync();
                }
            }
        }

        private async Task ExecuteFavoriteWorkshopCommand(WorkshopCellViewViewModel workshop)
        {
            if (workshop?.SelectedWorkshop == null)
                return;

            var response = await PageDialogService.DisplayAlertAsync("Unfavorite workshop",
                "Are you sure you want to remove this workshop from your favorites?",
                "Unfavorite",
                "Cancel");

            if (response)
            {
                var toggled = await FavoriteService.ToggleFavorite(workshop.SelectedWorkshop);
                if (toggled)
                {
                    await ExecuteLoadWorkshopsCommandAsync();
                }
            }
        }

        private void UpdatePage()
        {
            bool forceRefresh = (DateTime.UtcNow > (NextForceRefresh)) ||
                    LoggedIn != Settings.Email;

            LoggedIn = Settings.Email;
            if (forceRefresh)
            {
                RefreshCommand.Execute(null);
                FavoritesTime = Settings.LastFavoriteTime;
            }
            else
            {

                if (Tweets.Count == 0)
                {
                    LoadSocialCommand.Execute(null);
                }

                if ((_firstLoad && Sessions.Count == 0) || FavoritesTime != Settings.LastFavoriteTime)
                {
                    if (_firstLoad)
                        Settings.LastFavoriteTime = DateTime.UtcNow;

                    _firstLoad = false;
                    FavoritesTime = Settings.LastFavoriteTime;
                    LoadSessionsCommand.Execute(null);
                }
                if ((_firstLoad && Workshops.Count == 0) || FavoritesTime != Settings.LastFavoriteTime)
                {
                    if (_firstLoad)
                        Settings.LastFavoriteTime = DateTime.UtcNow;

                    _firstLoad = false;
                    FavoritesTime = Settings.LastFavoriteTime;
                    LoadWorkshopsCommand.Execute(null);
                }

                if (Notification == null)
                    LoadNotificationsCommand.Execute(null);
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (!parameters.ContainsKey("ComingBack") && !parameters.ContainsKey("Tab") && Settings.FirstRun && !Settings.IsLoggedIn)
            {
                GoToModalCommand.Execute(new List<string> { nameof(LoginPage) });
                return;
            }
            UpdatePage();
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
