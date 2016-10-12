using developer.open.space.DataStore.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Mock.Helpers
{
    public class Settings : developer.open.space.DataStore.Mock.Interfaces.ISettings
    {
        private ISessionStore _sessionStore;
        private ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public Settings(ISessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }


        public bool IsFavorite(string id) =>
            AppSettings.GetValueOrDefault<bool>("fav_" + id, false);

        public void SetFavorite(string id, bool favorite) =>
            AppSettings.AddOrUpdateValue("fav_" + id, favorite);

        public async Task ClearFavorites()
        {
            var sessions = await _sessionStore.GetItemsAsync();
            foreach (var session in sessions)
                AppSettings.Remove("fav_" + session.Id);
        }

        public bool LeftFeedback(string id) =>
        AppSettings.GetValueOrDefault<bool>("feed_" + id, false);

        public void LeaveFeedback(string id, bool leave) =>
        AppSettings.AddOrUpdateValue("feed_" + id, leave);

        public async Task ClearFeedback()
        {
            var sessions = await _sessionStore.GetItemsAsync();
            foreach (var session in sessions)
                AppSettings.Remove("feed_" + session.Id);
        }

    }
}
