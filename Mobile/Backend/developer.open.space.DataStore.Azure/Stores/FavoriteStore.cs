using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Azure.Stores
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
