using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions
{
    public interface IStoreManager
    {
        bool IsInitialized { get; }
        Task InitializeAsync();
        ICategoryStore CategoryStore { get; }
        IFavoriteStore FavoriteStore { get; }
        IFeedbackStore FeedbackStore { get; }
        ISessionStore SessionStore { get; }
        ISpeakerStore SpeakerStore { get; }
        ISponsorStore SponsorStore { get; }
        IEventStore EventStore { get; }
        INotificationStore NotificationStore { get; }
        IWorkshopStore WorkshopStore { get; }
        IApplicationDataStore ApplicationDataStore { get; }
        Task<bool> SyncAllAsync(bool syncUserSpecific);
        Task DropEverythingAsync();
        Task<MobileServiceUser> LoginAsync();
        Task LogoutAsync();

    }
}
