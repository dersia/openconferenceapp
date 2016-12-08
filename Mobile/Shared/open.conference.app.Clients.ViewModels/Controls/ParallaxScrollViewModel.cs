using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace open.conference.app.Clients.ViewModels
{
    public class ParallaxScrollViewModel : ViewModelBase
    {
        public ParallaxScrollViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
        }

        public static readonly BindableProperty ParallaxViewProperty =
            BindableProperty.Create(nameof(ParallaxView), typeof(View), typeof(ParallaxScrollViewModel), null);

        private View _parallaxView;
        public View ParallaxView
        {
            get { return _parallaxView; }
            set { SetProperty(ref _parallaxView, value); }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private double _scrollY;
        public double ScrollY
        {
            get { return _scrollY; }
            set { SetProperty(ref _scrollY, value); }
        }

        public ICommand ParallaxScroll => DelegateCommand.FromAsyncHandler(async () => await ParallaxAsync());

        public async Task ParallaxAsync()
        {
            if (ParallaxView == null || Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
                return;

            if (Height <= 0)
                Height = ParallaxView.Height;

            var y = -(int)((float)ScrollY / 2.5f);
            if (y < 0)
            {
                //Move the Image's Y coordinate a fraction of the ScrollView's Y position
                ParallaxView.Scale = 1;
                ParallaxView.TranslationY = y;
            }
            else if (Device.OS == TargetPlatform.iOS)
            {
                //Calculate a scale that equalizes the height vs scroll
                double newHeight = Height + (ScrollY * -1);
                ParallaxView.Scale = newHeight / Height;
                ParallaxView.TranslationY = -(ScrollY / 2);
            }
            else
            {
                ParallaxView.Scale = 1;
                ParallaxView.TranslationY = 0;
            }
        }
    }
}
