using Microsoft.Practices.Unity;
using open.conference.app.DataStore.Abstractions.DataObjects;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.Utils.Helpers;
using open.conference.app.Utils.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace open.conference.app.Clients.ViewModels.Models.Extensions
{
    public static class SessionCellViewExtension
    {
        public static IEnumerable<SessionCellViewViewModel> Search(this IEnumerable<SessionCellViewViewModel> sessionCellViews, string searchText = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return sessionCellViews;

            var searchSplit = searchText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //search title, then category, then speaker name
            return sessionCellViews.Where(sessionCellView =>
                                  searchSplit.Any(search => sessionCellView.SelectedSession != null &&
                                sessionCellView.SelectedSession.Haystack.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public static IEnumerable<IGrouping<string, SessionCellViewViewModel>> FilterAndGroupByDate(this IList<SessionCellViewViewModel> sessions)
        {
            if (Settings.Current.FavoritesOnly)
            {
                sessions = sessions.Where(s => s.SelectedSession != null && s.SelectedSession.IsFavorite).ToList();
            }

            var tba = sessions.Where(s => s.SelectedSession != null && (!s.SelectedSession.StartTime.HasValue || !s.SelectedSession.EndTime.HasValue || s.SelectedSession.StartTime.Value.IsTBA()));


            var showPast = Settings.Current.ShowPastSessions;
            var showAllCategories = Settings.Current.ShowAllCategories;
            var filteredCategories = Settings.Current.FilteredCategories;
            var utc = DateTime.UtcNow;


            //is not tba
            //has not started or has started and hasn't ended or ended 20 minutes ago
            //filter then by category and filters
            var grouped = (from session in sessions
                           where session.SelectedSession != null && session.SelectedSession.StartTime.HasValue && session.SelectedSession.EndTime.HasValue && !session.SelectedSession.StartTime.Value.IsTBA() && (showPast || (utc <= session.SelectedSession.StartTime.Value || utc <= session.SelectedSession.EndTime.Value.AddMinutes(20)))
                           && (showAllCategories || filteredCategories.IndexOf(session?.SelectedSession.MainCategory?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0)
                           orderby session.SelectedSession.StartTimeOrderBy, session.Title
                           group session by session.SelectedSession.GetSortName()
                            into sessionGroup
                           select sessionGroup).ToList();

            if (tba.Any())
                grouped.Add(new Grouping<string, SessionCellViewViewModel>("TBA", tba));

            return grouped;
        }

        public static IEnumerable<SessionCellViewViewModel> ToSessionCellViews(this IEnumerable<Session> sessions, Func<Session, SessionCellViewViewModel> createViewModelFactory)
        {
            return sessions?.Select(session => createViewModelFactory(session));            
        }
    }
}
