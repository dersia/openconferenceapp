using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Clients.ViewModels.Models;
using open.conference.app.Clients.Views;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.Utils.Helpers;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels
{
    public class ViewModelBase : BindableBase, IProvideToolbarItems, IProvideEffects
    {
        public ObservableRangeCollection<ToolbarItem> ToolBarItems { get; } = new ObservableRangeCollection<ToolbarItem>();
        public ObservableRangeCollection<string> Effects { get; } = new ObservableRangeCollection<string>();
        private bool _canLoadMore;
        public bool CanLoadMore { get { return _canLoadMore; } set { SetProperty(ref _canLoadMore, value); } }
        private string _icon;
        public string Icon { get { return _icon; } set { SetProperty(ref _icon, value); } }
        private bool _isBusy;
        public bool IsBusy { get { return _isBusy; } set { SetProperty(ref _isBusy, value); } }
        public bool IsNotBusy { get { return !_isBusy; } set { SetProperty(ref _isBusy, !value); } }
        private string _subtitle;
        public string Subtitle { get { return _subtitle; } set { SetProperty(ref _subtitle, value); } }
        private string _title;
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        protected INavigationService Navigation { get; }
        protected ILoggerFacade Logger { get; }
        protected IStoreManager StoreManager { get; }
        protected IToast Toast { get; }
        protected IFavoriteService FavoriteService { get; }
        protected ISSOClient SSOClient { get; }
        protected IEventAggregator EventAggregator { get; }
        protected IPushNotifications PushNotifications { get; }
        protected IReminderService ReminderService { get; }
        protected IPageDialogService PageDialogService { get; } 
        public ILaunchTwitter LaunchTwitter { get; }

        public ViewModelBase(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
        {
            Navigation = navigationService;
            EventAggregator = eventAggregator;
            StoreManager = storeManager;
            Toast = toast;
            FavoriteService = favoriteService;
            Logger = logger;
            SSOClient = ssoClient;
            PushNotifications = pushNotifications;
            ReminderService = reminderService;
            PageDialogService = pageDialogService;
            LaunchTwitter = twitter;

        }

        public Settings Settings
        {
            get { return Settings.Current; }
        }

        public ICommand LaunchBrowserCommand => DelegateCommand<string>.FromAsyncHandler(async (t) => await ExecuteLaunchBrowserAsync(t));

        public ICommand GoToCommand => DelegateCommand<IList<string>>.FromAsyncHandler(pages => GoTo(pages));
        public ICommand GoToModalCommand => DelegateCommand< IList<string>>.FromAsyncHandler(pages => GoTo(pages, true));

        private async Task GoTo(IList<string> pageName, bool isModal = false)
        {
            await Navigation.NavigateAsync(NavigationUriFactory.RelativeUri(pageName.ToArray()), useModalNavigation: isModal);
        }

        private async Task ExecuteLaunchBrowserAsync(string arg)
        {
            if (IsBusy)
                return;

            if (!arg.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !arg.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                arg = "http://" + arg;

            Logger.Log($"{DevopenspaceLoggerKeys.LaunchedBrowser}, Url, {arg}",Category.Info, Priority.None);

            var lower = arg.ToLowerInvariant();
            if (Device.OS == TargetPlatform.iOS && lower.Contains("twitter.com"))
            {
                try
                {
                    var id = arg.Substring(lower.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    if (lower.Contains("/status/"))
                    {
                        //status
                        if (await LaunchTwitter.OpenStatus(id))
                            return;
                    }
                    else
                    {
                        //user
                        if (await LaunchTwitter.OpenUserName(id))
                            return;
                    }
                }
                catch
                {
                }
            }

            try
            {
                await CrossShare.Current.OpenBrowser(arg, new BrowserOptions
                {
                    ChromeShowTitle = true,
                    ChromeToolbarColor = new ShareColor
                    {
                        A = 255,
                        R = 118,
                        G = 53,
                        B = 235
                    },
                    UseSafariWebViewController = true
                });
            }
            catch
            {
            }
        }
    }
}
