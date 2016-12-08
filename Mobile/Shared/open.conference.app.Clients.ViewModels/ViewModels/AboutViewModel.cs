using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Clients.ViewModels.Models;
using open.conference.app.Clients.Views;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace open.conference.app.Clients.ViewModels
{
    public class AboutViewModel : SettingsViewModel, INavigationAware, IProvideToolbarItems
    {
        private bool _isRegistered;
        public ObservableRangeCollection<IGrouping<string, Models.MenuItem>> MenuItems { get; }
        public ObservableRangeCollection<Models.MenuItem> InfoItems { get; } = new ObservableRangeCollection<Models.MenuItem>();
        public ObservableRangeCollection<Models.MenuItem> AccountItems { get; } = new ObservableRangeCollection<Models.MenuItem>();
        public new ObservableRangeCollection<ToolbarItem> ToolBarItems { get; } = new ObservableRangeCollection<ToolbarItem>();

        private Models.MenuItem syncItem;
        private Models.MenuItem accountItem;
        private Models.MenuItem pushItem;
        public AboutViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            ToolBarItems.Add(new ToolbarItem
            {
                Text = LoginText,
                Command = LoginCommand
            });
            AboutItems.Clear();
            AboutItems.Add(new Models.MenuItem { Name = "About this app", Icon = "icon_venue.png" });

            InfoItems.AddRange(new[] 
            {
                new Models.MenuItem { Name = "Conference Feed", Icon = "menu_feed.png", Pagename = nameof(FeedPage) },
                new Models.MenuItem { Name = "Sponsors", Icon = "menu_sponsors.png", Pagename = nameof(SponsorsPage) },
                new Models.MenuItem { Name = "Venue", Icon = "menu_venue.png", Pagename = nameof(VenuePage) },
                new Models.MenuItem { Name = "Floor Maps", Icon = "menu_plan.png", Pagename = nameof(FloorMapsPage) },
                new Models.MenuItem { Name = "Conference Info", Icon = "menu_info.png", Pagename = nameof(ConferenceInformationPage) },
                new Models.MenuItem { Name = "Settings", Icon = "menu_settings.png", Pagename = nameof(SettingsPage) }
            });

            accountItem = new Models.MenuItem
            {
                Name = "Logged in as:"
            };

            syncItem = new Models.MenuItem
            {
                Name = "Last Sync:"
            };

            pushItem = new Models.MenuItem
            {
                Name = "Enable push notifications"
            };

            pushItem.Command = DelegateCommand.FromAsyncHandler(async () =>
            {
                if (PushNotifications.IsRegistered)
                {
                    UpdateItems();
                    return;
                }

                if (Settings.AttemptedPush)
                {
                    var response = await PageDialogService.DisplayAlertAsync("Push Notification",
                        "To enable push notifications, please go into Settings, Tap Notifications, and set Allow Notifications to on.",
                        "Settings",
                        "Maybe Later");

                    if (response)
                    {
                        PushNotifications.OpenSettings();
                    }
                }

                await PushNotifications.RegisterForNotifications();
            });

            UpdateItems();

            AccountItems.Add(accountItem);
            AccountItems.Add(syncItem);
            AccountItems.Add(pushItem);

            //This will be triggered wen 
            Settings.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Email" || e.PropertyName == "LastSync" || e.PropertyName == "PushNotificationsEnabled")
                {
                    UpdateItems();
                    OnPropertyChanged("AccountItems");
                }
            };

            AccountItems.CollectionChanged += (sender, e) =>
            {
                AccountListHeightAdjustment = AccountItems.Count;
            };
            AccountListHeightAdjustment = AccountItems.Count;

            _isRegistered = PushNotifications.IsRegistered;
        }

        private int _accountListHeightAdjustment;
        public int AccountListHeightAdjustment
        {
            get { return _accountListHeightAdjustment; }
            set { SetProperty(ref _accountListHeightAdjustment, value); }
        }

        private Models.MenuItem _selectedAccount;
        public Models.MenuItem SelectedAccount
        {
            get { return _selectedAccount; }
            set { SetProperty(ref _selectedAccount, value); }
        }

        private Models.MenuItem _selectedInfoItem;
        public Models.MenuItem SelectedInfoItem
        {
            get { return _selectedInfoItem; }
            set { SetProperty(ref _selectedInfoItem, value); if(_selectedInfoItem != null) GoToCommand.Execute(new List<string> { _selectedInfoItem.Pagename }); }
        }

        public ICommand IgnoreAccountTapCommand => new DelegateCommand<ItemTappedEventArgs>((args) => SelectedAccount = null);

        public override ICommand AboutItemSelectedCommand => new DelegateCommand<Models.MenuItem>(GoToSettings);

        private void GoToSettings(Models.MenuItem menuItem)
        {
            Logger.Log(AppPage.Settings.ToString(), Category.Info, Priority.None);
            GoToCommand.Execute(new List<string> { nameof(SettingsPage) });
        }

        public void UpdateItems()
        {
            syncItem.Subtitle = LastSyncDisplay;
            accountItem.Subtitle = Settings.IsLoggedIn ? Settings.UserDisplayName : "Not signed in";

            pushItem.Name = PushNotifications.IsRegistered ? "Push notifications enabled" : "Enable push notifications";
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (!_isRegistered && Settings.AttemptedPush)
            {
                PushNotifications.RegisterForNotifications();
            }
            _isRegistered = PushNotifications.IsRegistered;
            UpdateItems();
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
