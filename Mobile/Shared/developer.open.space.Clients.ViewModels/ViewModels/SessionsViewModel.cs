using Microsoft.Practices.Unity;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Clients.ViewModels.Models;
using developer.open.space.Utils.Helpers.Extensions;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using developer.open.space.Clients.ViewModels.Models.Extensions;
using System.Collections.Generic;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class SessionsViewModel : ViewModelBase, INavigationAware
    {
        private IUnityContainer _container;
        
        public ObservableRangeCollection<SessionCellViewViewModel> Sessions { get; } = new ObservableRangeCollection<SessionCellViewViewModel>();
        public ObservableRangeCollection<SessionCellViewViewModel> SessionsFiltered { get; } = new ObservableRangeCollection<SessionCellViewViewModel>();
        public ObservableRangeCollection<IGrouping<string, SessionCellViewViewModel>> SessionsGrouped { get; } = new ObservableRangeCollection<IGrouping<string, SessionCellViewViewModel>>();
        public DateTime NextForceRefresh { get; set; }

        private string _loggedIn;
        public string LoggedIn
        {
            get { return _loggedIn; }
            set { SetProperty(ref _loggedIn, value); }
        }

        private bool _showAllCategory;
        public bool ShowAllCategory
        {
            get { return _showAllCategory; }
            set { SetProperty(ref _showAllCategory, value); }
        }

        private bool _showPastCategory;
        public bool ShowPastCategory
        {
            get { return _showPastCategory; }
            set { SetProperty(ref _showPastCategory, value); }
        }
        private bool _showFavoritesCategory;
        public bool ShowFavoritesCategory
        {
            get { return _showFavoritesCategory; }
            set { SetProperty(ref _showFavoritesCategory, value); }
        }
        private string _filteredCategories;
        public string FilteredCategories
        {
            get { return _filteredCategories; }
            set { SetProperty(ref _filteredCategories, value); }
        }


        public SessionsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, IUnityContainer container)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            _container = container;
            NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
            ShowFavoritesCategory = Settings.FavoritesOnly;
            ShowPastCategory = Settings.ShowPastSessions;
            ShowAllCategory = Settings.ShowAllCategories;
            LoggedIn = Settings.Email;
            FilteredCategories = Settings.FilteredCategories;

            Setup();
            Sessions.CollectionChanged += (sender, args) => { OnPropertyChanged("Sessions"); };
        }


        private void Setup()
        {
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

        #region Properties
        SessionCellViewViewModel _selectedSession;
        public SessionCellViewViewModel SelectedSession
        {
            get { return _selectedSession; }
            set { SetProperty(ref _selectedSession, value); }
        }

        string filter = string.Empty;
        public string Filter
        {
            get { return filter; }
            set
            {
                if (SetProperty(ref filter, value))
                    Task.Run(async() => await ExecuteFilterSessionsAsync()).ConfigureAwait(false);
            }
        }

        #endregion

        #region Filtering and Sorting

        void SortSessions()
        {
            SessionsGrouped.ReplaceRange(SessionsFiltered.FilterAndGroupByDate());
        }

        bool noSessionsFound;
        public bool NoSessionsFound
        {
            get { return noSessionsFound; }
            set { SetProperty(ref noSessionsFound, value); }
        }

        string noSessionsFoundMessage;
        public string NoSessionsFoundMessage
        {
            get { return noSessionsFoundMessage; }
            set { SetProperty(ref noSessionsFoundMessage, value); }
        }

        #endregion

        #region Commands
        
        public ICommand GoToSessionDetailsCommand => new DelegateCommand<ItemTappedEventArgs>((args) => { if (args.Item != null && args.Item is SessionCellViewViewModel && ((SessionCellViewViewModel)args.Item).SelectedSession != null) GoToSession(((SessionCellViewViewModel)args.Item).SelectedSession); });

        private void GoToSession(Session selectedSession)
        {
            GoToCommand.Execute(new List<string> { nameof(SessionDetailsPage) });
            EventAggregator.GetEvent<SessionSelectedEvent>().Publish(selectedSession);
        }

        public ICommand ForceRefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteForceRefreshCommandAsync());

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExecuteLoadSessionsAsync(true);
        }

        public ICommand FilterSessionsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteFilterSessionsAsync());

        async Task ExecuteFilterSessionsAsync()
        {
            IsBusy = true;
            NoSessionsFound = false;

            // Abort the current command if the query has changed and is not empty
            if (!string.IsNullOrEmpty(Filter))
            {
                var query = Filter;
                await Task.Delay(250);
                if (query != Filter)
                    return;
            }

            SessionsFiltered.ReplaceRange(Sessions.Search(Filter));
            SortSessions();

            if (SessionsGrouped.Count == 0)
            {
                if (Settings.FavoritesOnly)
                {
                    if (!Settings.ShowPastSessions && !string.IsNullOrWhiteSpace(Filter))
                        NoSessionsFoundMessage = "You haven't favorited\nany sessions yet.";
                    else
                        NoSessionsFoundMessage = "No Sessions Found";
                }
                else
                    NoSessionsFoundMessage = "No Sessions Found";

                NoSessionsFound = true;
            }
            else
            {
                NoSessionsFound = false;
            }

            IsBusy = false;
        }

        public ICommand LoadSessionsCommand => DelegateCommand<bool>.FromAsyncHandler(async (f) => await ExecuteLoadSessionsAsync());

        private SessionCellViewViewModel SessionCellViewModelFactory(Session session)
        {
            return new SessionCellViewViewModel(Navigation, EventAggregator, StoreManager, Toast, FavoriteService, Logger, LaunchTwitter, SSOClient, PushNotifications, ReminderService, PageDialogService)
            {
                SelectedSession = session,
                FavoriteCommand = FavoriteCommand
            };
        }

        async Task<bool> ExecuteLoadSessionsAsync(bool force = false)
        {
            if (IsBusy)
                return false;

            try
            {
                NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
                IsBusy = true;
                NoSessionsFound = false;
                Filter = string.Empty;

                Sessions.ReplaceRange((await StoreManager.SessionStore.GetItemsAsync(force)).ToSessionCellViews(SessionCellViewModelFactory));

                SessionsFiltered.ReplaceRange(Sessions.Search());
                SortSessions();

                if (SessionsGrouped.Count == 0)
                {

                    if (Settings.FavoritesOnly)
                    {
                        if (!Settings.ShowPastSessions)
                            NoSessionsFoundMessage = "You haven't favorited\nany sessions yet.";
                        else
                            NoSessionsFoundMessage = "No Sessions Found";
                    }
                    else
                        NoSessionsFoundMessage = "No Sessions Found";

                    NoSessionsFound = true;
                }
                else
                {
                    NoSessionsFound = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadSessionsAsync", Prism.Logging.Category.Exception, Priority.High);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }
        
        public ICommand FavoriteCommand => DelegateCommand<SessionCellViewViewModel>.FromAsyncHandler(ExecuteFavoriteCommandAsync);

        private async Task ExecuteFavoriteCommandAsync(SessionCellViewViewModel session)
        {
            var toggled = await FavoriteService.ToggleFavorite(session.SelectedSession);
            if (toggled && Settings.FavoritesOnly)
                SortSessions();
        }

        public ICommand ShowFiltersCommand => new DelegateCommand(ShowFilters);

        private void ShowFilters()
        {
            if (IsBusy)
                return;
            Logger.Log(AppPage.Filter.ToString(), Prism.Logging.Category.Info, Priority.None);
            GoToCommand.Execute(new List<string> { nameof(FilterSessionsPage) });
        }



        #endregion
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            UpdatePage();
        }

        private void UpdatePage()
        {
            Title = Settings.FavoritesOnly ? "Favorite Sessions" : "Sessions";

            bool forceRefresh = (DateTime.UtcNow > NextForceRefresh) ||
                LoggedIn != Settings.Email;

            LoggedIn = Settings.Email;
            //Load if none, or if 45 minutes has gone by
            if (Sessions.Count == 0 || forceRefresh)
            {
                LoadSessionsCommand.Execute(forceRefresh);
            }
            else if (ShowFavoritesCategory != Settings.FavoritesOnly ||
                    ShowPastCategory != Settings.ShowPastSessions ||
                    ShowAllCategory != Settings.ShowAllCategories ||
                    FilteredCategories != Settings.FilteredCategories)
            {
                ShowFavoritesCategory = Settings.FavoritesOnly;
                ShowPastCategory = Settings.ShowPastSessions;
                ShowAllCategory = Settings.ShowAllCategories;
                FilteredCategories = Settings.FilteredCategories;
                FilterSessionsCommand.Execute(null);
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
