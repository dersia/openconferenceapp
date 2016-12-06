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
    public class WorkshopsViewModel : ViewModelBase, INavigationAware
    {
        private IUnityContainer _container;
        
        public ObservableRangeCollection<WorkshopCellViewViewModel> Workshops { get; } = new ObservableRangeCollection<WorkshopCellViewViewModel>();
        public ObservableRangeCollection<WorkshopCellViewViewModel> WorkshopsFiltered { get; } = new ObservableRangeCollection<WorkshopCellViewViewModel>();
        public ObservableRangeCollection<IGrouping<string, WorkshopCellViewViewModel>> WorkshopsGrouped { get; } = new ObservableRangeCollection<IGrouping<string, WorkshopCellViewViewModel>>();
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


        public WorkshopsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, IUnityContainer container)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            _container = container;
            NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
            ShowFavoritesCategory = Settings.FavoritesOnly;
            ShowPastCategory = Settings.ShowPastWorkshops;
            ShowAllCategory = Settings.ShowAllCategories;
            LoggedIn = Settings.Email;
            FilteredCategories = Settings.FilteredCategories;

            Setup();
            Workshops.CollectionChanged += (sender, args) => { OnPropertyChanged("Workshops"); };
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
        WorkshopCellViewViewModel _selectedWorkshop;
        public WorkshopCellViewViewModel SelectedWorkshop
        {
            get { return _selectedWorkshop; }
            set { SetProperty(ref _selectedWorkshop, value); }
        }

        string filter = string.Empty;
        public string Filter
        {
            get { return filter; }
            set
            {
                if (SetProperty(ref filter, value))
                    Task.Run(async() => await ExecuteFilterWorkshopsAsync()).ConfigureAwait(false);
            }
        }

        #endregion

        #region Filtering and Sorting

        void SortWorkshops()
        {
            WorkshopsGrouped.ReplaceRange(WorkshopsFiltered.FilterAndGroupByDate());
        }

        bool noWorkshopsFound;
        public bool NoWorkshopsFound
        {
            get { return noWorkshopsFound; }
            set { SetProperty(ref noWorkshopsFound, value); }
        }

        string noWorkshopsFoundMessage;
        public string NoWorkshopsFoundMessage
        {
            get { return noWorkshopsFoundMessage; }
            set { SetProperty(ref noWorkshopsFoundMessage, value); }
        }

        #endregion

        #region Commands

        public ICommand GoToWorkshopDetailsCommand => new DelegateCommand<ItemTappedEventArgs>((args) => { if (args.Item != null && args.Item is WorkshopCellViewViewModel && ((WorkshopCellViewViewModel)args.Item).SelectedWorkshop != null) GoToWorkshop(((WorkshopCellViewViewModel)args.Item).SelectedWorkshop); });

        private void GoToWorkshop(Workshop selectedWorkshop)
        {
            GoToCommand.Execute(new List<string> { nameof(WorkshopDetailsPage) });
            EventAggregator.GetEvent<WorkshopSelectedEvent>().Publish(selectedWorkshop);
        }

        public ICommand ForceRefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteForceRefreshCommandAsync());

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExecuteLoadWorkshopsAsync(true);
        }

        public ICommand FilterWorkshopsCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteFilterWorkshopsAsync());

        async Task ExecuteFilterWorkshopsAsync()
        {
            IsBusy = true;
            NoWorkshopsFound = false;

            // Abort the current command if the query has changed and is not empty
            if (!string.IsNullOrEmpty(Filter))
            {
                var query = Filter;
                await Task.Delay(250);
                if (query != Filter)
                    return;
            }

            WorkshopsFiltered.ReplaceRange(Workshops.Search(Filter));
            SortWorkshops();

            if (WorkshopsGrouped.Count == 0)
            {
                if (Settings.FavoritesOnly)
                {
                    if (!Settings.ShowPastWorkshops && !string.IsNullOrWhiteSpace(Filter))
                        NoWorkshopsFoundMessage = "You haven't favorited\nany workshops yet.";
                    else
                        NoWorkshopsFoundMessage = "No Workshops Found";
                }
                else
                    NoWorkshopsFoundMessage = "No Workshops Found";

                NoWorkshopsFound = true;
            }
            else
            {
                NoWorkshopsFound = false;
            }

            IsBusy = false;
        }

        public ICommand LoadWorkshopsCommand => DelegateCommand<bool>.FromAsyncHandler(async (f) => await ExecuteLoadWorkshopsAsync());

        private WorkshopCellViewViewModel WorkshopCellViewModelFactory(Workshop workshop)
        {
            return new WorkshopCellViewViewModel(Navigation, EventAggregator, StoreManager, Toast, FavoriteService, Logger, LaunchTwitter, SSOClient, PushNotifications, ReminderService, PageDialogService)
            {
                SelectedWorkshop = workshop,
                FavoriteCommand = FavoriteCommand
            };
        }

        async Task<bool> ExecuteLoadWorkshopsAsync(bool force = false)
        {
            if (IsBusy)
                return false;

            try
            {
                NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
                IsBusy = true;
                NoWorkshopsFound = false;
                Filter = string.Empty;

                Workshops.ReplaceRange((await StoreManager.WorkshopStore.GetItemsAsync(force)).ToWorkshopCellViews(WorkshopCellViewModelFactory));

                WorkshopsFiltered.ReplaceRange(Workshops.Search());
                SortWorkshops();

                if (WorkshopsGrouped.Count == 0)
                {

                    if (Settings.FavoritesOnly)
                    {
                        if (!Settings.ShowPastWorkshops)
                            NoWorkshopsFoundMessage = "You haven't favorited\nany workshops yet.";
                        else
                            NoWorkshopsFoundMessage = "No Workshops Found";
                    }
                    else
                        NoWorkshopsFoundMessage = "No Workshops Found";

                    NoWorkshopsFound = true;
                }
                else
                {
                    NoWorkshopsFound = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadWorkshopsAsync", Prism.Logging.Category.Exception, Priority.High);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }
        
        public ICommand FavoriteCommand => DelegateCommand<WorkshopCellViewViewModel>.FromAsyncHandler(ExecuteFavoriteCommandAsync);

        private async Task ExecuteFavoriteCommandAsync(WorkshopCellViewViewModel workshop)
        {
            var toggled = await FavoriteService.ToggleFavorite(workshop.SelectedWorkshop);
            if (toggled && Settings.FavoritesOnly)
                SortWorkshops();
        }

        public ICommand ShowFiltersCommand => DelegateCommand.FromAsyncHandler(ShowFilters);

        private async Task ShowFilters()
        {
            if (IsBusy)
                return;
            Logger.Log(AppPage.Filter.ToString(), Prism.Logging.Category.Info, Priority.None);
            GoToCommand.Execute(new List<string> { nameof(FilterWorkshopsPage) });
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
            Title = Settings.FavoritesOnly ? "Favorite Workshops" : "Workshops";

            bool forceRefresh = (DateTime.UtcNow > NextForceRefresh) ||
                LoggedIn != Settings.Email;

            LoggedIn = Settings.Email;
            //Load if none, or if 45 minutes has gone by
            if (Workshops.Count == 0 || forceRefresh)
            {
                LoadWorkshopsCommand.Execute(forceRefresh);
            }
            else if (ShowFavoritesCategory != Settings.FavoritesOnly ||
                    ShowPastCategory != Settings.ShowPastWorkshops ||
                    ShowAllCategory != Settings.ShowAllCategories ||
                    FilteredCategories != Settings.FilteredCategories)
            {
                ShowFavoritesCategory = Settings.FavoritesOnly;
                ShowPastCategory = Settings.ShowPastWorkshops;
                ShowAllCategory = Settings.ShowAllCategories;
                FilteredCategories = Settings.FilteredCategories;
                FilterWorkshopsCommand.Execute(null);
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
