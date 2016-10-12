using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.Utils.Helpers;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class FloorMapsViewModel : ViewModelBase, INavigationAware
    {
        public ObservableRangeCollection<FloorImage> FloorMaps { get; } = new ObservableRangeCollection<FloorImage>();
        public FloorMapsViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, IUnityContainer container)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {            
        }

        private FloorImage _selectedFloor;
        public FloorImage SelectedFloor
        {
            get { return _selectedFloor; }
            set { SetProperty(ref _selectedFloor, value); }
        }

        public ICommand PreviewImageCommand => new DelegateCommand<FloorImage>((image) => SelectedFloor = image);

        private async Task LoadFloors()
        {
            if (IsBusy)
                return;

            try
            {
                var jsonUrl = await StoreManager.ApplicationDataStore.GetApplicationData(ApplicationDataConst.ApplicationDataFloorMapsJsonUrl);
                if (jsonUrl != null && CrossConnectivity.Current.IsConnected)
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);
                        var json = await client.GetStringAsync(jsonUrl.Value);
                        var floorMaps = JsonConvert.DeserializeObject<IList<FloorMap>>(json);
                        if(floorMaps != null && floorMaps.Any())
                        {
                            FloorMaps.Clear();
                            FloorMaps.AddRange(floorMaps.Select(floorMap => new FloorImage
                            {
                                ImageUrl = floorMap.ImageUrl,
                                Image = ImageSource.FromUri(new Uri(floorMap.ImageUrl)),
                                ImageTitle = floorMap.Name
                            }));
                            OnPropertyChanged(nameof(FloorMaps));
                            SelectedFloor = FloorMaps[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            SelectedFloor = null;
            await LoadFloors();
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
