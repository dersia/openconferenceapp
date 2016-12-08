using open.conference.app.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace open.conference.app.DataStore.Mock
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

        public bool IsInitialized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public StoreManager(INotificationStore notificationStore, ICategoryStore categoryStore, IFavoriteStore favoriteStore, ISessionStore sessionStore, ISpeakerStore speakerStore, IEventStore eventStore, ISponsorStore sponsorStore, IFeedbackStore feedbackStore, IWorkshopStore workshopStore, IApplicationDataStore applicationDataStore)
        {
            _notificationStore = notificationStore;
            _categoryStore = categoryStore;
            _favoriteStore = favoriteStore;
            _sessionStore = sessionStore;
            _speakerStore = speakerStore;
            _eventStore = eventStore;
            _sponsorStore = sponsorStore;
            _feedbackStore = feedbackStore;
            _workshopStore = workshopStore;
            _applicationDataStore = applicationDataStore;
        }

        #region IStoreManager implementation

        public Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            return Task.FromResult(true);
        }

        #endregion

        public Task DropEverythingAsync()
        {
            return Task.FromResult(true);
        }

        public Task<MobileServiceUser> LoginAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
