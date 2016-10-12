using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Mock.Stores
{
    public class NotificationStore : BaseStore<Notification>, INotificationStore
    {
        public NotificationStore()
        {
        }

        public async Task<Notification> GetLatestNotification()
        {
            var items = await GetItemsAsync();
            return items.ElementAt(0);
        }

        public override Task<IEnumerable<Notification>> GetItemsAsync(bool forceRefresh = false)
        {
            var items = new[]
            {
                new Notification
                {
                    Date = DateTime.UtcNow,
                    Text = "Welcome to Developer Open Space 2016!"
                }
            };
            return Task.FromResult(items as IEnumerable<Notification>);
        }
    }
}
