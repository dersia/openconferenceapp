using developer.open.space.Clients.ViewModels.Interfaces;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class SponsorsViewModel : ViewModelBase, INavigationAware
    {
        public SponsorsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
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
            if (Device.OS == TargetPlatform.Android)
                Effects.Add("developer.open.space.ListViewSelectionOnTopEffect");
        }

        public ObservableRangeCollection<Sponsor> Sponsors { get; } = new ObservableRangeCollection<Sponsor>();
        public ObservableRangeCollection<IGrouping<string, Sponsor>> SponsorsGrouped { get; } = new ObservableRangeCollection<IGrouping<string, Sponsor>>();

        #region Properties

        Sponsor _selectedSponsor;
        public Sponsor SelectedSponsor
        {
            get { return _selectedSponsor; }
            set { SetProperty(ref _selectedSponsor, value); }
        }

        public ICommand SponsorTappedCommand => new DelegateCommand<ItemTappedEventArgs>((eventArgs) => GoToSposnorDetails(eventArgs?.Item as Sponsor));

        private void GoToSposnorDetails(Sponsor sponsor)
        {
            if (sponsor == null)
                return;

            GoToCommand.Execute(new List<string> { nameof(SponsorDetailsPage) });
            EventAggregator.GetEvent<SponsorSelectedEvent>().Publish(sponsor);
        }

        public ToolbarItem AdditionalToolbarItem
        {
            get
            {
                if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
                {
                    return new ToolbarItem
                    {
                        Text = "Refresh",
                        Icon = "toolbar_refresh.png",
                        Command = ForceRefreshCommand
                    };
                }

                return null;
            }
        }

        #endregion

        #region Sorting

        void SortSponsors(IEnumerable<Sponsor> sponsors)
        {
            var sponsorsRanked = from sponsor in sponsors
                                 orderby sponsor.Name, sponsor.Rank
                                 orderby sponsor.SponsorLevel.Rank
                                 select sponsor;

            Sponsors.ReplaceRange(sponsorsRanked);

            var groups = from sponsor in Sponsors
                         group sponsor by sponsor.SponsorLevel.Name
                into sponsorGroup
                         select new Grouping<string, Sponsor>(sponsorGroup.Key, sponsorGroup);

            SponsorsGrouped.ReplaceRange(groups);
        }

        #endregion

        #region Commands
        public ICommand ForceRefreshCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteForceRefreshCommandAsync());

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExecuteLoadSponsorsAsync(true);
        }

        public ICommand LoadSponsorsCommand => DelegateCommand<bool>.FromAsyncHandler(ExecuteLoadSponsorsAsync);

        private async Task<bool> ExecuteLoadSponsorsAsync(bool force = false)
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;

                var sponsors = await StoreManager.SponsorStore.GetItemsAsync(force);

                SortSponsors(sponsors);

            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}, Method, ExecuteLoadSponsorsAsync", Prism.Logging.Category.Exception, Priority.High);
                EventAggregator.GetEvent<ErrorEvent>().Publish(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            await ExecuteLoadSponsorsAsync(false);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
