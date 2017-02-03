using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class FavoriteStore : BaseStore<Favorite>, IFavoriteStore
    {
        public FavoriteStore() : base() { }
        public async Task<bool> IsFavorite(string sessionId)
        {
            var items = await Table.Where(s => s.SessionId == sessionId).ToListAsync().ConfigureAwait(false);
            return items.Count > 0;
        }

        public async Task<bool> IsFavoriteWorkshop(string workshopId)
        {
            var items = await Table.Where(s => s.WorkshopId == workshopId).ToListAsync().ConfigureAwait(false);
            return items.Count > 0;
        }

        public Task DropFavorites()
        {
            return Task.FromResult(true);
        }

        public override string Identifier => "Favorite";
    }
}
