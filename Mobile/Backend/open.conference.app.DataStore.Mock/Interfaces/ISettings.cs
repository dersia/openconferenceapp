using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Mock.Interfaces
{
    public interface ISettings
    {
        bool IsFavorite(string id);
        void SetFavorite(string id, bool favorite);
        Task ClearFavorites();
        bool LeftFeedback(string id);
        void LeaveFeedback(string id, bool leave);
        Task ClearFeedback();
    }
}
