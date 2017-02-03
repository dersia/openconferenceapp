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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace open.conference.app.Clients.ViewModels
{
    public class DroidRootViewModel : ViewModelBase
    {
        private ObservableRangeCollection<MenuItem> _pages = new ObservableRangeCollection<MenuItem>();

        public DroidRootViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            SetupPages();
            _selectedMenuItem = Pages[0];
        }

        private MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { SetProperty(ref _selectedMenuItem, value); if (_selectedMenuItem != null) GoToCommand.Execute(new List<string> { nameof(SimpleNavigationPage), _selectedMenuItem.Pagename }); }
        }

        public ObservableRangeCollection<MenuItem> Pages
        {
            get { return _pages; }
        }

        private void SetupPages()
        {
            Pages.AddRange(new List<MenuItem>
            {
                new MenuItem
                {
                    Name = "Conference Feed",
                    Icon = "menu_feed.png",
                    Pagename = nameof(FeedPage)
                },
                new MenuItem
                {
                    Name = "Sessions",
                    Icon = "menu_sessions.png",
                    Pagename = nameof(SessionsPage)
                },
                new MenuItem
                {
                    Name = "Workshops",
                    Icon = "menu_sessions.png",
                    Pagename = nameof(WorkshopsPage)
                },
                new MenuItem
                {
                    Name = "Events",
                    Icon = "menu_events.png",
                    Pagename = nameof(EventsPage)
                },
                new MenuItem
                {
                    Name = "Sponsors",
                    Icon = "menu_sponsors.png",
                    Pagename = nameof(SponsorsPage)
                },
                new MenuItem
                {
                    Name = "Venue",
                    Icon = "menu_venue.png",
                    Pagename = nameof(VenuePage)
                },
                new MenuItem
                {
                    Name = "Floor Maps",
                    Icon = "menu_plan.png",
                    Pagename = nameof(FloorMapsPage)
                },
                new MenuItem
                {
                    Name = "Conference Info",
                    Icon = "menu_info.png",
                    Pagename = nameof(ConferenceInformationPage)
                },
                new MenuItem
                {
                    Name = "Settings",
                    Icon = "menu_settings.png",
                    Pagename = nameof(SettingsPage)
                }
            });
        }
    }
}
