using Microsoft.Practices.Unity;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Clients.ViewModels.Models;
using developer.open.space.Clients.Views;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class iOSRootViewModel : ViewModelBase, INavigationAware, IProvideTabs
    {
        private Microsoft.Practices.Unity.IUnityContainer _container;

        public iOSRootViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, Microsoft.Practices.Unity.IUnityContainer container)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            _container = container;
        }

        public ObservableRangeCollection<Xamarin.Forms.Page> Tabs { get; } = new ObservableRangeCollection<Xamarin.Forms.Page>();

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
