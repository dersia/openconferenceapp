using Microsoft.Practices.Unity;
using open.conference.app.Clients.ViewModels;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Clients.ViewModels.ViewModels;
using open.conference.app.Clients.Views;
using open.conference.app.DataStore.Abstractions;
using Prism.Logging;
using Prism.Unity;
using Xamarin.Forms;
using Prism.Mvvm;
using System;
using System.Linq;
using Prism.Unity.Navigation;
using Prism.Common;
using open.conference.app.Utils.Helpers;

namespace open.conference.app.Clients.Application
{
    public partial class OpenConferenceApplication : PrismApplication
    {
        private bool _mock = false;
        private bool _isDesktop = false;
        public OpenConferenceApplication(IPlatformInitializer platformInitializer, bool isDesktop = false, bool mock = true) : base(platformInitializer)
        {
            _mock = mock;
            _isDesktop = isDesktop;
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeComponent();
        }

        protected override async void OnInitialized()
        {
            var storeManager = Container.Resolve<IStoreManager>();
            if (storeManager != null)
            {
                await storeManager.InitializeAsync();
            }
            Startup();
        }

        protected override void RegisterTypes()
        {
            RegisterServices();
            RegisterViews();
            RegisterViewModels();
        }

        private async void Startup()
        {
            Uri navigationUri = null;
            switch(Device.OS)
            {
                case TargetPlatform.Android:
                    navigationUri = NavigationUriFactory.AbsoluteUri(nameof(DroidRootPage), nameof(SimpleNavigationPage), nameof(FeedPage));
                    break;
                case TargetPlatform.iOS:
                    navigationUri = NavigationUriFactory.AbsoluteUri(nameof(iOSRootPage), nameof(SimpleNavigationPage), nameof(FeedPage));
                    break;
                case TargetPlatform.Windows:
                case TargetPlatform.WinPhone:
                    navigationUri = NavigationUriFactory.AbsoluteUri(nameof(UWPRootPage), nameof(SimpleNavigationPage), nameof(FeedPage));
                    break;
                case TargetPlatform.Other:
                default:
                    break;
            }
            if(navigationUri != null)
            {
                await NavigationService.NavigateAsync(navigationUri);
            }            
        }
        
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region RegisterServices
        private void RegisterServices()
        {
            if (_mock)
            {
                Container.RegisterType<ISessionStore, open.conference.app.DataStore.Mock.Stores.SessionStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IWorkshopStore, open.conference.app.DataStore.Mock.Stores.WorkshopStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IFavoriteStore, open.conference.app.DataStore.Mock.Stores.FavoriteStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IFeedbackStore, open.conference.app.DataStore.Mock.Stores.FeedbackStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISpeakerStore, open.conference.app.DataStore.Mock.Stores.SpeakerStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISponsorStore, open.conference.app.DataStore.Mock.Stores.SponsorStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ICategoryStore, open.conference.app.DataStore.Mock.Stores.CategoryStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IEventStore, open.conference.app.DataStore.Mock.Stores.EventStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<INotificationStore, open.conference.app.DataStore.Mock.Stores.NotificationStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IApplicationDataStore, open.conference.app.DataStore.Mock.Stores.ApplicationDataStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISSOClient, Services.MockSSOClient>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IStoreManager, open.conference.app.DataStore.Mock.StoreManager>(new ContainerControlledLifetimeManager());
            }
            else
            {
                Container.RegisterType<ISessionStore, open.conference.app.DataStore.Azure.Stores.SessionStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IWorkshopStore, open.conference.app.DataStore.Azure.Stores.WorkshopStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IFavoriteStore, open.conference.app.DataStore.Azure.Stores.FavoriteStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IFeedbackStore, open.conference.app.DataStore.Azure.Stores.FeedbackStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISpeakerStore, open.conference.app.DataStore.Azure.Stores.SpeakerStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISponsorStore, open.conference.app.DataStore.Azure.Stores.SponsorStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ICategoryStore, open.conference.app.DataStore.Azure.Stores.CategoryStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IEventStore, open.conference.app.DataStore.Azure.Stores.EventStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<INotificationStore, open.conference.app.DataStore.Azure.Stores.NotificationStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IApplicationDataStore, open.conference.app.DataStore.Azure.Stores.ApplicationDataStore>(new ContainerControlledLifetimeManager());
                Container.RegisterType<ISSOClient, open.conference.app.Clients.Application.Services.Azure.AzureSSOClient>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IStoreManager, open.conference.app.DataStore.Azure.StoreManager>(new ContainerControlledLifetimeManager());
            }

            Container.RegisterType<IFavoriteService, open.conference.app.Clients.Application.Services.FavoriteService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IReminderService, Services.ReminderService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILoggerFacade, DebugLogger>(new ContainerControlledLifetimeManager());
        }
        #endregion

        #region RegisterViews
        private void RegisterViews()
        {
            Container.RegisterTypeForNavigation<EventsPage, EventsViewModel>();
            Container.RegisterTypeForNavigation<EventDetailsPage, EventDetailsViewModel>();
            Container.RegisterTypeForNavigation<FeedPage, FeedViewModel>();
            Container.RegisterTypeForNavigation<NotificationsPage, NotificationsViewModel>();
            Container.RegisterTypeForNavigation<TweetImagePage, TweetImageViewModel>();
            Container.RegisterTypeForNavigation<CodeOfConductPage, ConferenceInfoViewModel>();
            Container.RegisterTypeForNavigation<ConferenceInformationPage, ConferenceInfoViewModel>();
            Container.RegisterTypeForNavigation<FloorMapsPage, FloorMapsViewModel>();
            Container.RegisterTypeForNavigation<LoginPage, LoginViewModel>();
            Container.RegisterTypeForNavigation<SettingsPage, SettingsViewModel>();
            Container.RegisterTypeForNavigation<VenuePage, VenueViewModel>();
            Container.RegisterTypeForNavigation<WiFiInformationPage, ConferenceInfoViewModel>();
            Container.RegisterTypeForNavigation<FilterSessionsPage, FilterSessionsViewModel>();
            Container.RegisterTypeForNavigation<SessionDetailsPage, SessionDetailsViewModel>();
            Container.RegisterTypeForNavigation<SessionsPage, SessionsViewModel>();
            Container.RegisterTypeForNavigation<FilterWorkshopsPage, FilterWorkshopsViewModel>();
            Container.RegisterTypeForNavigation<WorkshopDetailsPage, WorkshopDetailsViewModel>();
            Container.RegisterTypeForNavigation<WorkshopsPage, WorkshopsViewModel>();
            Container.RegisterTypeForNavigation<SpeakerDetailsPage, SpeakerDetailsViewModel>();
            Container.RegisterTypeForNavigation<SponsorDetailsPage, SponsorDetailsViewModel>();
            Container.RegisterTypeForNavigation<SponsorsPage, SponsorsViewModel>();
            Container.RegisterTypeForNavigation<AboutPage, AboutViewModel>();
            Container.RegisterTypeForNavigation<UWPRootPage, UWPRootViewModel>();
            Container.RegisterTypeForNavigation<UWPMenuPage, UWPRootViewModel>();
            Container.RegisterTypeForNavigation<DroidRootPage, DroidRootViewModel>();
            Container.RegisterTypeForNavigation<iOSRootPage, iOSRootViewModel>();
            Container.RegisterTypeForNavigation<SimpleNavigationPage, SimpleNavigationViewModel>();
            //Container.RegisterTypeForNavigation<FeedbackPage, FeedbackViewModel>();
        }
        #endregion

        #region RegisterViewModels
        private void RegisterViewModels()
        {
            Container.RegisterType<CategoryCellViewViewModel, CategoryCellViewViewModel>(new TransientLifetimeManager());
            Container.RegisterType<SessionCellViewViewModel, SessionCellViewViewModel>(new TransientLifetimeManager());
            Container.RegisterType<WorkshopCellViewViewModel, WorkshopCellViewViewModel>(new TransientLifetimeManager());
        }
        #endregion

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null)
                {
                    var navService = Container.Resolve<UnityPageNavigationService>();
                    ((IPageAware)navService).Page = page;

                    overrides = new ParameterOverrides
                    {
                        { "navigationService", navService }
                    };
                }
                
                return Container.Resolve(type, overrides);
            });
            ViewModelLocationProvider.SetDefaultViewModelFactory(ResolveViewModel);

        }

        protected override IUnityContainer CreateContainer()
        {
            var con = base.CreateContainer();
            
            return con;
        }

        private object ResolveViewModel(Type viewType)
        {
            return Container.Resolve(viewType);
        }
    }
}
