using open.conference.app.DataStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Mock.Stores
{
    public class BaseStore<T> : IBaseStore<T>
    {
        public BaseStore()
        {
        }

        #region IBaseStore implementation

        public void DropTable()
        {

        }
        public virtual System.Threading.Tasks.Task InitializeStore()
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<T> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<bool> InsertAsync(T item)
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<bool> UpdateAsync(T item)
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<bool> RemoveAsync(T item)
        {
            throw new NotImplementedException();
        }
        public virtual System.Threading.Tasks.Task<bool> SyncAsync()
        {
            return Task.FromResult(true);
        }

        public string Identifier => "store";
        #endregion
    }
}
