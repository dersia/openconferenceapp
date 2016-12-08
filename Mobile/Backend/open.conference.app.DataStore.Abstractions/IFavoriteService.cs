using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavorite(Session session);
        Task<bool> ToggleFavorite(Workshop workshop);
    }
}
