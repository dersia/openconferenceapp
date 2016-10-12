using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.Utils.Helpers;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class VenueViewModel : ViewModelBase, IProvidePins, IMoveMap
    {
        public ObservableRangeCollection<Pin> Pins { get; } = new ObservableRangeCollection<Pin>();
        public MapSpan MapSpan { get; }
        public bool CanMakePhoneCall => CrossMessaging.Current.PhoneDialer.CanMakePhoneCall;
        public string EventTitle => "Developer Open Space 2016";
        public string LocationTitle => "Commundo Tagungshotel Leipzig";
        public string Address1 => "Zschochersche Straße 69";
        public string Address2 => "04229 Leipzig";
        public double Latitude => 51.327152;
        public double Longitude => 12.335797;

        public VenueViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            if (Device.OS == TargetPlatform.Android)
            {
                ToolBarItems.Add(new ToolbarItem
                {
                    Order = ToolbarItemOrder.Secondary,
                    Text = "Get Directions",
                    Command = NavigateCommand

                });

                if (CanMakePhoneCall)
                {

                    ToolBarItems.Add(new ToolbarItem
                    {
                        Order = ToolbarItemOrder.Secondary,
                        Text = "Call Hotel",
                        Command = CallCommand
                    });
                }
            }
            else if (Device.OS == TargetPlatform.iOS)
            {
                ToolBarItems.Add(new ToolbarItem
                {
                    Text = "More",
                    Icon = "toolbar_overflow.png",
                    Command = DelegateCommand<object>.FromAsyncHandler(async (o) =>
                    {
                        string[] items = null;
                        if (!CanMakePhoneCall)
                        {
                            items = new[] { "Get Directions" };
                        }
                        else
                        {
                            items = new[] { "Get Directions", "Call +49 (341) 4859270" };
                        }
                        var action = await PageDialogService.DisplayActionSheetAsync("Commundo Tagungshotel", "Cancel", null, items);
                        if (action == items[0])
                            NavigateCommand.Execute(null);
                        else if (items.Length > 1 && action == items[1] && CanMakePhoneCall)
                            CallCommand.Execute(null);

                    })
                });
            }
            else
            {
                ToolBarItems.Add(new ToolbarItem
                {
                    Text = "Directions",
                    Command = NavigateCommand,
                    Icon = "toolbar_navigate.png"
                });

                if (CanMakePhoneCall)
                {

                    ToolBarItems.Add(new ToolbarItem
                    {
                        Text = "Call",
                        Command = CallCommand,
                        Icon = "toolbar_call.png"
                    });
                }
            }
            var position = new Position(Latitude, Longitude);
            MapSpan = new MapSpan(position, 0.02, 0.02);
            Pins.Add(new Pin
            {
                Type = PinType.Place,
                Address = LocationTitle,
                Label = EventTitle,
                Position = position
            });
        }
        
        public ICommand NavigateCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteNavigateCommandAsync());

        async Task ExecuteNavigateCommandAsync()
        {
            Logger.Log(DevopenspaceLoggerKeys.NavigateToDevopenspace, Category.Exception, Priority.None);
            if (!await CrossExternalMaps.Current.NavigateTo(LocationTitle, Latitude, Longitude))
            {
                await PageDialogService.DisplayAlertAsync("Unable to Navigate",
                    "Please ensure that you have a map application installed.",
                    "OK");
            }
        }
        
        public ICommand CallCommand => DelegateCommand.FromAsyncHandler(ExecuteCallCommand);

        private async Task ExecuteCallCommand()
        {
            Logger.Log(DevopenspaceLoggerKeys.CallHotel, Category.Exception, Priority.None);
            var phoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (phoneCallTask.CanMakePhoneCall)
                phoneCallTask.MakePhoneCall("+493414859270");
        }
    }
}
