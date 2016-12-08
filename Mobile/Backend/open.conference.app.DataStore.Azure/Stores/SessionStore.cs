using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class SessionStore : BaseStore<Session>, ISessionStore
    {
        private IFavoriteStore _favoriteStore;
        public SessionStore(IFavoriteStore favoriteStore) : base()
        {
            _favoriteStore = favoriteStore;
        }
        public override async Task<IEnumerable<Session>> GetItemsAsync(bool forceRefresh = false)
        {
            var sessions = await GetSessions(forceRefresh);
            if(!sessions.Any())
            {
                sessions = await GetSessions(true);
            }
            await _favoriteStore.GetItemsAsync(true).ConfigureAwait(false);//pull latest

            foreach (var session in sessions)
            {
                var isFav = await _favoriteStore.IsFavorite(session.Id).ConfigureAwait(false);
                session.IsFavorite = isFav;
            }

            return sessions;
        }

        private async Task<IEnumerable<Session>> GetSessions(bool forceRefresh)
        {
            if (forceRefresh)
                await PullLatestAsync().ConfigureAwait(false);

            return await Table.OrderBy(s => s.StartTime).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId)
        {
            var speakers = await GetItemsAsync().ConfigureAwait(false);

            return speakers.Where(s => s.Speakers != null && s.Speakers.Any(speak => speak.Id == speakerId))
                .OrderBy(s => s.StartTimeOrderBy);
        }

        public async Task<IEnumerable<Session>> GetNextSessions()
        {
            var date = DateTime.UtcNow.AddMinutes(-30);//about to start in next 30

            var sessions = await GetItemsAsync().ConfigureAwait(false);

            var result = sessions.Where(s => s.StartTimeOrderBy > date && s.IsFavorite).Take(2);

            var enumerable = result as Session[] ?? result.ToArray();
            return enumerable.Any() ? enumerable : null;
        }

        public async Task<Session> GetAppIndexSession(string id)
        {
            var sessions = await Table.Where(s => s.Id == id || s.RemoteId == id).ToListAsync();

            if (sessions == null || sessions.Count == 0)
                return null;

            return sessions[0];
        }

        public override string Identifier => "Session";
    }
}
