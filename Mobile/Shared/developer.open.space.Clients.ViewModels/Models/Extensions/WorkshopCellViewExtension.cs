using Microsoft.Practices.Unity;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.Utils.Helpers;
using developer.open.space.Utils.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace developer.open.space.Clients.ViewModels.Models.Extensions
{
    public static class WorkshopCellViewExtension
    {
        public static IEnumerable<WorkshopCellViewViewModel> Search(this IEnumerable<WorkshopCellViewViewModel> workshopCellViews, string searchText = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return workshopCellViews;

            var searchSplit = searchText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //search title, then category, then speaker name
            return workshopCellViews.Where(workshopCellView =>
                                  searchSplit.Any(search => workshopCellView.SelectedWorkshop != null &&
                                workshopCellView.SelectedWorkshop.Haystack.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public static IEnumerable<IGrouping<string, WorkshopCellViewViewModel>> FilterAndGroupByDate(this IList<WorkshopCellViewViewModel> workshops)
        {
            if (Settings.Current.FavoritesOnly)
            {
                workshops = workshops.Where(s => s.SelectedWorkshop != null && s.SelectedWorkshop.IsFavorite).ToList();
            }

            var tba = workshops.Where(s => s.SelectedWorkshop != null && (!s.SelectedWorkshop.StartTime.HasValue || !s.SelectedWorkshop.EndTime.HasValue || s.SelectedWorkshop.StartTime.Value.IsTBA()));


            var showPast = Settings.Current.ShowPastWorkshops;
            var showAllCategories = Settings.Current.ShowAllCategories;
            var filteredCategories = Settings.Current.FilteredCategories;
            var utc = DateTime.UtcNow;


            //is not tba
            //has not started or has started and hasn't ended or ended 20 minutes ago
            //filter then by category and filters
            var grouped = (from workshop in workshops
                           where workshop.SelectedWorkshop != null && workshop.SelectedWorkshop.StartTime.HasValue && workshop.SelectedWorkshop.EndTime.HasValue && !workshop.SelectedWorkshop.StartTime.Value.IsTBA() && (showPast || (utc <= workshop.SelectedWorkshop.StartTime.Value || utc <= workshop.SelectedWorkshop.EndTime.Value.AddMinutes(20)))
                           && (showAllCategories || filteredCategories.IndexOf(workshop?.SelectedWorkshop.MainCategory?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0)
                           orderby workshop.SelectedWorkshop.StartTimeOrderBy, workshop.Title
                           group workshop by workshop.SelectedWorkshop.GetSortName()
                            into workshopGroup
                           select workshopGroup).ToList();

            if (tba.Any())
                grouped.Add(new Grouping<string, WorkshopCellViewViewModel>("TBA", tba));

            return grouped;
        }

        public static IEnumerable<WorkshopCellViewViewModel> ToWorkshopCellViews(this IEnumerable<Workshop> workshops, Func<Workshop, WorkshopCellViewViewModel> createViewModelFactory)
        {
            return workshops?.Select(workshop => createViewModelFactory(workshop));            
        }
    }
}
