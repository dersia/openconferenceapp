using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.DataStore.Abstractions.PubSubEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class TweetImageViewModel : ViewModelBase, INavigationAware
    {
        public TweetImageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            EventAggregator.GetEvent<TweetImageChangedEvent>().Subscribe(ImageChanged);
            var item = new ToolbarItem
            {
                Text = "Done",
                Command = DelegateCommand.FromAsyncHandler(async () => await Navigation.GoBackAsync())
            };

            if (Device.OS == TargetPlatform.Android)
                item.Icon = "toolbar_close.png";
            ToolBarItems.Add(item);
        }
        ~TweetImageViewModel()
        {
            EventAggregator.GetEvent<TweetImageChangedEvent>().Unsubscribe(ImageChanged);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private UriImageSource _tweetImage;
        public UriImageSource TweetImage
        {
            get { return _tweetImage; }
            set { SetProperty(ref _tweetImage, value); }
        }

        private void ImageChanged(string image)
        {
            try
            {
                TweetImage = new UriImageSource
                {
                    Uri = new Uri(image),
                    CachingEnabled = true,
                    CacheValidity = TimeSpan.FromDays(3)
                };
            }
            catch (Exception ex)
            {
                Logger.Log("Unable to convert image to URI: " + ex.ToString(), Category.Exception, Priority.High);
                Toast.SendToast("Unable to load image.");
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
