using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions
{
    public interface IFavoriteStore : IBaseStore<Favorite>
    {
        Task<bool> IsFavorite(string sessionId);
        Task<bool> IsFavoriteWorkshop(string workshopId);
        Task DropFavorites();
    }
}

