using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IBaseStore<T>
    {
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        Task<T> GetItemAsync(string id);
        Task<bool> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> RemoveAsync(T item);
        Task<bool> SyncAsync();

        void DropTable();

        string Identifier { get; }
    }
}
