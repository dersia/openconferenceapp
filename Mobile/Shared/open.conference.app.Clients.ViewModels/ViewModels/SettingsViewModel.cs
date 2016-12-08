using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Clients.ViewModels.Models;
using open.conference.app.Clients.Views;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.DataStore.Abstractions.PubSubEvents;
using open.conference.app.Utils.Helpers;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Models.MenuItem> AboutItems { get; } = new ObservableRangeCollection<Models.MenuItem>();
        public ObservableRangeCollection<Models.MenuItem> TechnologyItems { get; } = new ObservableRangeCollection<Models.MenuItem>();

        public SettingsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            //This will be triggered wen 
            Settings.PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == "Email")
                {
                    Settings.NeedsSync = true;
                    OnPropertyChanged("LoginText");
                    OnPropertyChanged("AccountItems");
                    //if logged in you should go ahead and sync data.
                    if (Settings.IsLoggedIn)
                    {
                        await ExecuteSyncCommandAsync();
                    }
                }
            };

            AboutItems.AddRange(new[]
                {
                    new Models.MenuItem { Name = "Created by MyMie with love for Dev Open Space Leipzig", Command=LaunchBrowserCommand, Parameter="https://www.mymieapp.com" },
                    new Models.MenuItem { Name = "Open source on GitHub!", Command=LaunchBrowserCommand, Parameter="https://github.com/dersia/devopenspaceapp"},
                    //new Models.MenuItem { Name = "Open Source Notice", Command=LaunchBrowserCommand, Parameter="http://tiny.cc/app-evolve-osn"}
                });

            TechnologyItems.AddRange(new[]
                {
                    new Models.MenuItem { Name = "Azure Mobile Apps", Command=LaunchBrowserCommand, Parameter="https://github.com/Azure/azure-mobile-apps-net-client/" },
                    new Models.MenuItem { Name = "Censored", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Censored"},
                    new Models.MenuItem { Name = "Calendar Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/TheAlmightyBob/Calendars"},
                    new Models.MenuItem { Name = "Connectivity Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Connectivity"},
                    new Models.MenuItem { Name = "Embedded Resource Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/JosephHill/EmbeddedResourcePlugin"},
                    new Models.MenuItem { Name = "External Maps Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/ExternalMaps"},
                    new Models.MenuItem { Name = "Image Circles", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/ImageCircle"},
                    new Models.MenuItem { Name = "Json.NET", Command=LaunchBrowserCommand, Parameter="https://github.com/JamesNK/Newtonsoft.Json"},
                    new Models.MenuItem { Name = "LinqToTwitter", Command=LaunchBrowserCommand, Parameter="https://github.com/JoeMayo/LinqToTwitter"},
                    new Models.MenuItem { Name = "Messaging Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/cjlotz/Xamarin.Plugins"},
                    new Models.MenuItem { Name = "Permissions Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Permissions"},
                    new Models.MenuItem { Name = "Xamarin Evolve App", Command=LaunchBrowserCommand, Parameter="https://github.com/xamarinhq/app-evolve"},
                    new Models.MenuItem { Name = "PCL Storage", Command=LaunchBrowserCommand, Parameter="https://github.com/dsplaisted/PCLStorage"},
                    new Models.MenuItem { Name = "Pull to Refresh Layout", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Forms-PullToRefreshLayout"},
                    new Models.MenuItem { Name = "Settings Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings"},
                    new Models.MenuItem { Name = "Toolkit for Xamarin.Forms", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/xamarin.forms-toolkit"},
                    new Models.MenuItem { Name = "Xamarin.Forms", Command=LaunchBrowserCommand, Parameter="http://xamarin.com/forms"},
                    new Models.MenuItem { Name = "Prism.Forms", Command=LaunchBrowserCommand, Parameter="https://github.com/PrismLibrary/Prism"},
                });

            AboutItems.CollectionChanged += (sender, e) =>
            {
                AboutListHeightAdjustment = AboutItems.Count;
            };
            AboutListHeightAdjustment = AboutItems.Count;
        }

        private int _aboutListHeightAdjustment;
        public int AboutListHeightAdjustment
        {
            get { return _aboutListHeightAdjustment; }
            set { SetProperty(ref _aboutListHeightAdjustment, value); }
        }

        private int _technologyListHeightAdjustment;
        public int TechnologyListHeightAdjustment
        {
            get { return _technologyListHeightAdjustment; }
            set { SetProperty(ref _technologyListHeightAdjustment, value); }
        }

        private Models.MenuItem _selectedAboutItem;
        public Models.MenuItem SelectedAboutItem
        {
            get { return _selectedAboutItem; }
            set { SetProperty(ref _selectedAboutItem, value); if(_selectedAboutItem != null) AboutItemSelectedCommand.Execute(_selectedAboutItem); }
        }

        private Models.MenuItem _selectedTechonologyItem;
        public Models.MenuItem SelectedTechnologyItem
        {
            get { return _selectedTechonologyItem; }
            set { SetProperty(ref _selectedTechonologyItem, value); }
        }

        public ICommand IgnoreAboutTapCommand => new DelegateCommand<ItemTappedEventArgs>((item) => SelectedAboutItem = null);
        public ICommand IgnoreTechonologyTapCommand => new DelegateCommand<ItemTappedEventArgs>((item) => SelectedTechnologyItem = null);
        public ICommand TappedCommand => DelegateCommand.FromAsyncHandler(ShowCredits);

        public virtual ICommand AboutItemSelectedCommand => DelegateCommand<Models.MenuItem>.FromAsyncHandler(async (item) => { await Task.FromResult(0); });

        private async Task ShowCredits()
        {
            var sb = new StringBuilder();

            sb.AppendLine("The Developer Open Space mobile apps were handcrafted by MyMie");
            sb.AppendLine();
            sb.AppendLine("Development:");
            sb.AppendLine("Siavash Ghassemi");
            sb.AppendLine("Bosko Kovacevic");
            sb.AppendLine();
            sb.AppendLine("Inspired by:");
            sb.AppendLine("Xamarin Evolve App");
            sb.AppendLine();
            sb.AppendLine("Testing:");
            sb.AppendLine("Kjell Otto");
            sb.AppendLine("Sven Sönnichsen");
            sb.AppendLine("Torsten Weber");
            sb.AppendLine("Gregor Woiwode");
            sb.AppendLine("Robin Sedlaczek");
            sb.AppendLine();
            sb.AppendLine("Many thanks to:");
            sb.AppendLine("Xamarin for the Evolve App");
            sb.AppendLine("The Dev Open Space Community");
            sb.AppendLine("");
            sb.AppendLine("...and of course all the users!");

            await PageDialogService.DisplayAlertAsync("Credits", sb.ToString(), "OK");
        }

        public string LoginText => Settings.IsLoggedIn ? "Sign Out" : "Sign In";

        public string LastSyncDisplay
        {
            get
            {
                if (!Settings.HasSyncedData)
                    return "Never";

                return Settings.LastSync.ToString();//.Humanize();
            }
        }

        public ICommand LoginCommand => DelegateCommand.FromAsyncHandler(ExecuteLoginCommand);

        private async Task ExecuteLoginCommand()
        {
            if (IsBusy)
                return;

            if (!CrossConnectivity.Current.IsConnected)
            {
                EventAggregator.GetEvent<OfflineEvent>().Publish();
                return;
            }


            if (Settings.IsLoggedIn)
            {
                var response = await PageDialogService.DisplayAlertAsync("Logout?",
                    "Are you sure you want to logout? You can only save favorites and leave feedback when logged in.",
                    "Yes, Logout", "Cancel");
                if (response)
                {
                    await Logout();
                }

                return;
            }

            Logger.Log($"{AppPage.Login.ToString()}, Settings", Category.Info, Priority.None);
            GoToModalCommand.Execute(new List<string> { nameof(LoginPage) });
        }

        private async Task Logout()
        {
            Logger.Log(DevopenspaceLoggerKeys.Logout, Category.Info, Priority.None);

            try
            {
                if (SSOClient != null)
                {
                    await SSOClient.LogoutAsync();
                }

                Settings.FirstName = string.Empty;
                Settings.LastName = string.Empty;
                Settings.Email = string.Empty; //this triggers login text changed!

                //drop favorites and feedback because we logged out.
                await StoreManager.FavoriteStore.DropFavorites();
                await StoreManager.FeedbackStore.DropFeedback();
                await StoreManager.DropEverythingAsync();
                await ExecuteSyncCommandAsync();
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteLoginCommandAsync";
                Logger.Log(ex.Message, Category.Exception, Priority.High);
            }
        }

        string syncText = "Last sync";
        public string SyncText
        {
            get { return syncText; }
            set { SetProperty(ref syncText, value); }
        }

        public ICommand SyncCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteSyncCommandAsync());

        public object MessagingUtils { get; private set; }

        private async Task ExecuteSyncCommandAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                EventAggregator.GetEvent<OfflineEvent>().Publish();
                return;
            }

            Logger.Log(DevopenspaceLoggerKeys.ManualSync, Category.Info, Priority.High);

            SyncText = "Syncing...";

            try
            {

                Settings.HasSyncedData = true;
                Settings.LastSync = DateTime.UtcNow;
                OnPropertyChanged("LastSyncDisplay");

                await StoreManager.SyncAllAsync(Settings.Current.IsLoggedIn);
                if (!Settings.Current.IsLoggedIn)
                {
                    await PageDialogService.DisplayAlertAsync("Developer Open Space Data Synced",
                        "You now have the latest conference data, however to sync your favorites you must sign in with your Twitter account.",
                        "OK");
                }

            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteSyncCommandAsync";
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
                Logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            finally
            {
                SyncText = "Last sync";
            }
        }
    }
}
