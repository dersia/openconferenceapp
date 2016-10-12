using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.Clients.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private Prism.Logging.ILoggerFacade _logger;
        Session sessionQueued;
        Workshop workshopQueued;
        private IFavoriteStore _favoriteStore;
        public FavoriteService(Prism.Logging.ILoggerFacade logger, IFavoriteStore favoriteStore)
        {
            _favoriteStore = favoriteStore;
            _logger = logger;
        }
        public async Task<bool> ToggleFavorite(Session session)
        {
            if (!Settings.Current.IsLoggedIn)
            {
                sessionQueued = session;
                return false;
            }

            sessionQueued = null;

            session.IsFavorite = !session.IsFavorite;//switch first so UI updates :)
            if (!session.IsFavorite)
            {
                _logger.Log($"{DevopenspaceLoggerKeys.FavoriteRemoved}, Title, {session.Title}", Prism.Logging.Category.Info, Prism.Logging.Priority.None);

                var items = await _favoriteStore.GetItemsAsync();
                foreach (var item in items.Where(s => s.SessionId == session.Id))
                {
                    await _favoriteStore.RemoveAsync(item);
                }
            }
            else
            {
                _logger.Log($"{DevopenspaceLoggerKeys.FavoriteAdded}, Title, {session.Title}", Prism.Logging.Category.Info, Prism.Logging.Priority.None);
                await _favoriteStore.InsertAsync(new Favorite { SessionId = session.Id });
            }

            Settings.Current.LastFavoriteTime = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> ToggleFavorite(Workshop workshop)
        {
            if (!Settings.Current.IsLoggedIn)
            {
                workshopQueued = workshop;
                return false;
            }

            workshopQueued = null;

            workshop.IsFavorite = !workshop.IsFavorite;//switch first so UI updates :)
            if (!workshop.IsFavorite)
            {
                _logger.Log($"{DevopenspaceLoggerKeys.FavoriteRemoved}, Title, {workshop.Title}", Prism.Logging.Category.Info, Prism.Logging.Priority.None);

                var items = await _favoriteStore.GetItemsAsync();
                foreach (var item in items.Where(s => s.WorkshopId == workshop.Id))
                {
                    await _favoriteStore.RemoveAsync(item);
                }
            }
            else
            {
                _logger.Log($"{DevopenspaceLoggerKeys.FavoriteAdded}, Title, {workshop.Title}", Prism.Logging.Category.Info, Prism.Logging.Priority.None);
                await _favoriteStore.InsertAsync(new Favorite { WorkshopId = workshop.Id });
            }

            Settings.Current.LastFavoriteTime = DateTime.UtcNow;
            return true;
        }
    }
}
