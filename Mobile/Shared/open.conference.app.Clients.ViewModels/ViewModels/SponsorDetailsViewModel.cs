using System;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.DataStore.Abstractions.PubSubEvents;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels.ViewModels
{
    public class SponsorDetailsViewModel : ViewModelBase, INavigationAware
    {
        private Sponsor _selectedSponsor;
        public Sponsor SelectedSponsor
        {
            get { return _selectedSponsor; }
            set { SetProperty(ref _selectedSponsor, value); }
        }
        public ObservableRangeCollection<Models.MenuItem> FollowItems { get; } = new ObservableRangeCollection<Models.MenuItem>();

        public SponsorDetailsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            EventAggregator.GetEvent<SponsorSelectedEvent>().Subscribe(SponsorSelected);
        }
        ~SponsorDetailsViewModel()
        {
            EventAggregator.GetEvent<SponsorSelectedEvent>().Unsubscribe(SponsorSelected);
        }

        private int _followsCount;
        public int FollowsCount
        {
            get { return _followsCount; }
            set { SetProperty(ref _followsCount, value); }
        }

        Models.MenuItem _selectedFollowItem;
        public Models.MenuItem SelectedFollowItem
        {
            get { return _selectedFollowItem; }
            set
            {
                SetProperty(ref _selectedFollowItem, value);
                if (_selectedFollowItem == null)
                    return;

                LaunchBrowserCommand.Execute(_selectedFollowItem.Parameter);

                SelectedFollowItem = null;
            }
        }
        private void SponsorSelected(Sponsor selectedSponsor)
        {
            SelectedSponsor = selectedSponsor;
            FollowItems.Add(new Models.MenuItem
            {
                Name = "Web",
                Subtitle = SelectedSponsor.WebsiteUrl,
                Parameter = SelectedSponsor.WebsiteUrl,
                Icon = "icon_website.png"
            });
            FollowItems.Add(new Models.MenuItem
            {
                Name = Device.OS == TargetPlatform.iOS ? "Twitter" : SelectedSponsor.TwitterUrl,
                Subtitle = $"@{SelectedSponsor.TwitterUrl}",
                Parameter = "http://twitter.com/" + SelectedSponsor.TwitterUrl,
                Icon = "icon_twitter.png"
            });

            FollowsCount = FollowItems.Count;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
