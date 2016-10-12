using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Azure
{
    public class StoreManager : IStoreManager
    {
        INotificationStore _notificationStore;
        public INotificationStore NotificationStore => _notificationStore;

        ICategoryStore _categoryStore;
        public ICategoryStore CategoryStore => _categoryStore;

        IFavoriteStore _favoriteStore;
        public IFavoriteStore FavoriteStore => _favoriteStore;

        IFeedbackStore _feedbackStore;
        public IFeedbackStore FeedbackStore => _feedbackStore;

        ISessionStore _sessionStore;
        public ISessionStore SessionStore => _sessionStore;

        IWorkshopStore _workshopStore;
        public IWorkshopStore WorkshopStore => _workshopStore;

        ISpeakerStore _speakerStore;
        public ISpeakerStore SpeakerStore => _speakerStore;

        IEventStore _eventStore;
        public IEventStore EventStore => _eventStore;

        ISponsorStore _sponsorStore;
        public ISponsorStore SponsorStore => _sponsorStore;

        IApplicationDataStore _applicationDataStore;
        public IApplicationDataStore ApplicationDataStore => _applicationDataStore;

        private IAuthenticate _authenticator;
        public IAuthenticate Authenticator => _authenticator;
        public static MobileServiceClient MobileService { get; set; }

        public StoreManager(INotificationStore notificationStore, ICategoryStore categoryStore, IFavoriteStore favoriteStore, ISessionStore sessionStore, ISpeakerStore speakerStore, IEventStore eventStore, ISponsorStore sponsorStore, IFeedbackStore feedbackStore, IAuthenticate authenticator, IWorkshopStore workshopStore, IApplicationDataStore applicationDataStore)
        {
            _notificationStore = notificationStore;
            _categoryStore = categoryStore;
            _favoriteStore = favoriteStore;
            _sessionStore = sessionStore;
            _speakerStore = speakerStore;
            _eventStore = eventStore;
            _sponsorStore = sponsorStore;
            _feedbackStore = feedbackStore;
            _authenticator = authenticator;
            _workshopStore = workshopStore;
            _applicationDataStore = applicationDataStore;
            Task.Run(async () => { await InitializeAsync().ConfigureAwait(false); }).Wait();
        }

        /// <summary>
        /// Syncs all tables.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="syncUserSpecific">If set to <c>true</c> sync user specific.</param>
        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            if (!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>>();
            taskList.Add(CategoryStore.SyncAsync());
            taskList.Add(NotificationStore.SyncAsync());
            taskList.Add(SpeakerStore.SyncAsync());
            taskList.Add(SessionStore.SyncAsync());
            taskList.Add(WorkshopStore.SyncAsync());
            taskList.Add(SponsorStore.SyncAsync());
            taskList.Add(EventStore.SyncAsync());
            taskList.Add(ApplicationDataStore.SyncAsync());


            if (syncUserSpecific)
            {
                taskList.Add(FeedbackStore.SyncAsync());
                taskList.Add(FavoriteStore.SyncAsync());
            }

            var successes = await Task.WhenAll(taskList).ConfigureAwait(false);
            return successes.Any(x => !x);//if any were a failure.
        }

        /// <summary>
        /// Drops all tables from the database and updated DB Id
        /// </summary>
        /// <returns>The everything async.</returns>
        public Task DropEverythingAsync()
        {
            Settings.UpdateDatabaseId();
            CategoryStore.DropTable();
            EventStore.DropTable();
            NotificationStore.DropTable();
            SessionStore.DropTable();
            WorkshopStore.DropTable();
            SpeakerStore.DropTable();
            SponsorStore.DropTable();
            FeedbackStore.DropTable();
            FavoriteStore.DropTable();
            ApplicationDataStore.DropTable();
            IsInitialized = false;
            return Task.FromResult(true);
        }




        public bool IsInitialized { get; private set; }
        #region IStoreManager implementation
        object locker = new object();
        public async Task InitializeAsync()
        {
            MobileServiceSQLiteStore store;
            lock (locker)
            {

                if (IsInitialized)
                    return;

                IsInitialized = true;
                var dbId = Settings.DatabaseId;
                var path = System.IO.Path.Combine(Settings.SqlitePath,$"syncstore{dbId}.db");
                MobileService = new MobileServiceClient("https://devopenspaceworkshop2016.azurewebsites.net");
                store = new MobileServiceSQLiteStore(path);
                store.DefineTable<Category>();
                store.DefineTable<Favorite>();
                store.DefineTable<Notification>();
                store.DefineTable<FeaturedEvent>();
                store.DefineTable<Feedback>();
                store.DefineTable<Room>();
                store.DefineTable<Session>();
                store.DefineTable<Workshop>();
                store.DefineTable<Speaker>();
                store.DefineTable<Sponsor>();
                store.DefineTable<SponsorLevel>();
                store.DefineTable<StoreSettings>();
                store.DefineTable<ApplicationData>();
            }

            try
            {
                await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler()).ConfigureAwait(false);

                await LoadCachedTokenAsync().ConfigureAwait(false);
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        


        #endregion

        public async Task<MobileServiceUser> LoginAsync()
        {
            if (!IsInitialized)
            {
                await InitializeAsync();
            }

            MobileServiceUser user = await _authenticator.Authenticate();

            await CacheToken(user);

            return user;
        }

        public async Task LogoutAsync()
        {
            if (!IsInitialized)
            {
                await InitializeAsync();
            }

            await MobileService.LogoutAsync();

            var settings = await ReadSettingsAsync();

            if (settings != null)
            {
                settings.AuthToken = string.Empty;
                settings.UserId = string.Empty;

                await SaveSettingsAsync(settings);
            }
        }

        async Task SaveSettingsAsync(StoreSettings settings) =>
            await MobileService.SyncContext.Store.UpsertAsync(nameof(StoreSettings), new[] { JObject.FromObject(settings) }, true);

        async Task<StoreSettings> ReadSettingsAsync() =>
            (await MobileService.SyncContext.Store.LookupAsync(nameof(StoreSettings), StoreSettings.StoreSettingsId))?.ToObject<StoreSettings>();


        async Task CacheToken(MobileServiceUser user)
        {
            var settings = new StoreSettings
            {
                UserId = user.UserId,
                AuthToken = user.MobileServiceAuthenticationToken
            };

            await SaveSettingsAsync(settings);
        }

        async Task LoadCachedTokenAsync()
        {
            StoreSettings settings = await ReadSettingsAsync();

            if (settings != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(settings.AuthToken) && JwtUtility.GetTokenExpiration(settings.AuthToken) > DateTime.UtcNow)
                    {
                        MobileService.CurrentUser = new MobileServiceUser(settings.UserId);
                        MobileService.CurrentUser.MobileServiceAuthenticationToken = settings.AuthToken;
                    }
                }
                catch (InvalidTokenException)
                {
                    settings.AuthToken = string.Empty;
                    settings.UserId = string.Empty;

                    await SaveSettingsAsync(settings);
                }
            }
        }

        public class StoreSettings
        {
            public const string StoreSettingsId = "store_settings";

            public StoreSettings()
            {
                Id = StoreSettingsId;
            }

            public string Id { get; set; }

            public string UserId { get; set; }

            public string AuthToken { get; set; }
        }
    }
}
