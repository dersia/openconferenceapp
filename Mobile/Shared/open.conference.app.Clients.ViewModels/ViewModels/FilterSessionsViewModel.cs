using Microsoft.Practices.Unity;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.Clients.Views;
using open.conference.app.Clients.Views.Cells;
using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels.ViewModels
{
    public class FilterSessionsViewModel : ViewModelBase
    {
        private IUnityContainer _container;
        private open.conference.app.DataStore.Abstractions.DataObjects.Category _allCategory;
        private open.conference.app.DataStore.Abstractions.DataObjects.Category _showPastCategory;
        private open.conference.app.DataStore.Abstractions.DataObjects.Category _showFavoritesCategory;

        public FilterSessionsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, IUnityContainer container)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            _container = container;
            _allCategory = new open.conference.app.DataStore.Abstractions.DataObjects.Category
            {
                Name = "All",
                IsEnabled = true,
                IsFiltered = Settings.ShowAllCategories
            };

            _allCategory.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsFiltered")
                    SetShowAllCategories(_allCategory.IsFiltered);
            };

            ToolBarItems.Add(new ToolbarItem
            {
                Text = "Done",
                Command = DoneCommand,
                Icon = Device.OS != TargetPlatform.iOS ? "toolbar_close.png" : null
            });
        }
       

        public ObservableRangeCollection<CategoryCell> Categories { get; } = new ObservableRangeCollection<CategoryCell>();
        public ObservableRangeCollection<CategoryCell> Filters { get; } = new ObservableRangeCollection<CategoryCell>();
        

        private void SetShowAllCategories(bool showAll)
        {
            Settings.ShowAllCategories = showAll;
            foreach (var categoryCell in Categories)
            {
                var category = (categoryCell?.View?.BindingContext as CategoryCellViewViewModel)?.Category as open.conference.app.DataStore.Abstractions.DataObjects.Category;
                if (category != null)
                {
                    category.IsEnabled = !Settings.ShowAllCategories;
                    category.IsFiltered = Settings.ShowAllCategories || Settings.FilteredCategories.Contains(category.Name);
                }
            }
        }

        public ICommand DoneCommand => DelegateCommand.FromAsyncHandler(async () => 
        {
            Settings.FavoritesOnly = _showFavoritesCategory.IsFiltered;
            Settings.ShowPastSessions = _showPastCategory.IsFiltered;
            Save();
            await Navigation.GoBackAsync();
        });

        public async Task LoadCategoriesAsync()
        {
            await SetupCategories();
            SetupFilters();

            if (DateTime.UtcNow > Settings.EndOfDevopenspace)
                _showPastCategory.IsEnabled = false;

            _showPastCategory.IsFiltered = Settings.ShowPastSessions;
            _showFavoritesCategory.IsFiltered = Settings.FavoritesOnly;

            Save();
        }

        private async Task SetupCategories()
        {
            Categories.Clear();

            var allCategoryViewModel = _container.Resolve<CategoryCellViewViewModel>();
            allCategoryViewModel.Category = _allCategory;

            Categories.Add(new CategoryCell
            {
                View = new CategoryCellView
                {
                    BindingContext = allCategoryViewModel
                }
            });

            var items = await StoreManager.CategoryStore.GetItemsAsync();
            try
            {
                if (!items.Any())
                    items = await StoreManager.CategoryStore.GetItemsAsync(true);
            }
            catch
            {
                items = await StoreManager.CategoryStore.GetItemsAsync(true);
            }

            foreach (var category in items.OrderBy(c => c.Name))
            {
                category.IsFiltered = Settings.ShowAllCategories || Settings.FilteredCategories.Contains(category.Name);
                category.IsEnabled = !Settings.ShowAllCategories;
                var categoryViewModel = _container.Resolve<CategoryCellViewViewModel>();
                categoryViewModel.Category = category;
                Categories.Add(new CategoryCell { View = new CategoryCellView { BindingContext = categoryViewModel } });
            }
        }

        private void SetupFilters()
        {
            Filters.Clear();
            _showPastCategory = new open.conference.app.DataStore.Abstractions.DataObjects.Category
            {
                Name = "Show Past Sessions",
                IsEnabled = true,
                ShortName = "Show Past Sessions",
                Color = Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone ? "#7635EB" : string.Empty

            };

            var showPastViewModel = _container.Resolve<CategoryCellViewViewModel>();
            showPastViewModel.Category = _showPastCategory;

            var showFavoritesCategory = new open.conference.app.DataStore.Abstractions.DataObjects.Category
            {
                Name = "Show Favorites Only",
                IsEnabled = true,
                ShortName = "Show Favorites Only",
                Color = Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone ? "#7635EB" : string.Empty

            };
            var showFavoritesViewModel = _container.Resolve<CategoryCellViewViewModel>();
            showFavoritesViewModel.Category = showFavoritesCategory;

            Filters.AddRange(new List<CategoryCell>
            {
                new CategoryCell
                {
                    View = new CategoryCellView
                    {
                        BindingContext = showPastViewModel
                    }
                },
                new CategoryCell
                {
                    View = new CategoryCellView
                    {
                        BindingContext = showFavoritesViewModel
                    }
                }
            });

        }


        public void Save()
        {
            Settings.FilteredCategories = string.Join("|", Categories?.Where(c => ((open.conference.app.DataStore.Abstractions.DataObjects.Category)(c?.View?.BindingContext as CategoryCellViewViewModel)?.Category).IsFiltered).Select(c => ((open.conference.app.DataStore.Abstractions.DataObjects.Category)(c?.View?.BindingContext as CategoryCellViewViewModel)?.Category).Name));
        }
    }
}
