using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class WorkshopStore : BaseStore<Workshop>, IWorkshopStore
    {
        private IFavoriteStore _favoriteStore;
        public WorkshopStore(IFavoriteStore favoriteStore) : base()
        {
            _favoriteStore = favoriteStore;
        }
        public override async Task<IEnumerable<Workshop>> GetItemsAsync(bool forceRefresh = false)
        {
            var workshops = await GetWorkshops(forceRefresh);
            if(!workshops.Any())
            {
                workshops = await GetWorkshops(true);
            }
            await _favoriteStore.GetItemsAsync(true).ConfigureAwait(false);//pull latest

            foreach (var workshop in workshops)
            {
                var isFav = await _favoriteStore.IsFavoriteWorkshop(workshop.Id).ConfigureAwait(false);
                workshop.IsFavorite = isFav;
            }

            return workshops;
        }

        private async Task<IEnumerable<Workshop>> GetWorkshops(bool forceRefresh)
        {
            if (forceRefresh)
                await PullLatestAsync().ConfigureAwait(false);

            return await Table.OrderBy(s => s.StartTime).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Workshop>> GetSpeakerWorkshopsAsync(string speakerId)
        {
            var speakers = await GetItemsAsync().ConfigureAwait(false);

            return speakers.Where(s => s.Speakers != null && s.Speakers.Any(speak => speak.Id == speakerId))
                .OrderBy(s => s.StartTimeOrderBy);
        }

        public async Task<IEnumerable<Workshop>> GetNextWorkshops()
        {
            var date = DateTime.UtcNow.AddMinutes(-30);//about to start in next 30

            var workshops = await GetItemsAsync().ConfigureAwait(false);

            var result = workshops.Where(s => s.StartTimeOrderBy > date && s.IsFavorite).Take(2);

            var enumerable = result as Workshop[] ?? result.ToArray();
            return enumerable.Any() ? enumerable : null;
        }

        public async Task<Workshop> GetAppIndexWorkshops(string id)
        {
            var workshops = await Table.Where(s => s.Id == id || s.RemoteId == id).ToListAsync();

            if (workshops == null || workshops.Count == 0)
                return null;

            return workshops[0];
        }

        public override string Identifier => "Workshops";
    }
}
