using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class ApplicationDataStore : BaseStore<ApplicationData>, IApplicationDataStore
    {
        public ApplicationDataStore() : base() { }

        public async Task<ApplicationData> GetApplicationData(string key)
        {
            var items = await GetItemsAsync(appData => appData.Key == key, true);
            return items.OrderByDescending(s => s.Key).FirstOrDefault();
        }

        public async Task<IEnumerable<ApplicationData>> GetItemsAsync(Func<ApplicationData, bool> whereClause, bool forceRefresh = false)
        {
            var items = await GetItemsAsync(forceRefresh);
            return items.Where(whereClause);
        }

        public override async Task<IEnumerable<ApplicationData>> GetItemsAsync(bool forceRefresh = false)
        {
            var server = await base.GetItemsAsync(forceRefresh).ConfigureAwait(false);
            if (server.Count() == 0)
            {
                var items = new[]
                    {
                    new ApplicationData
                    {
                        Key = "Test",
                        Value = "Don't forget to favorite your sessions so you are ready for Developer Open Space 2016!"
                    }
                };
                return items;
            }
            return server.OrderByDescending(s => s.Key);
        }

        public override string Identifier => "ApplicationData";
    }
}
