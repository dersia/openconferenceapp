using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Mock.Stores
{
    public class FavoriteStore : BaseStore<Favorite>, IFavoriteStore
    {
        private open.conference.app.DataStore.Mock.Interfaces.ISettings _settings;

        public FavoriteStore(open.conference.app.DataStore.Mock.Interfaces.ISettings settings)
        {
            _settings = settings;
        }

        public Task<bool> IsFavorite(string sessionId)
        {
            return Task.FromResult(_settings.IsFavorite(sessionId));
        }

        public Task<bool> IsFavoriteWorkshop(string workshopId)
        {
            return Task.FromResult(_settings.IsFavorite(workshopId));
        }

        public override Task<bool> InsertAsync(Favorite item)
        {
            _settings.SetFavorite(item.SessionId, true);
            return Task.FromResult(true);
        }

        public override Task<bool> RemoveAsync(Favorite item)
        {
            _settings.SetFavorite(item.SessionId, false);
            return Task.FromResult(true);
        }

        public async Task DropFavorites()
        {
            await _settings.ClearFavorites();
        }
    }
}
