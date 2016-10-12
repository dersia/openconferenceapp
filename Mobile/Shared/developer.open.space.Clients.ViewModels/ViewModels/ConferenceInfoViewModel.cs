using Newtonsoft.Json;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.Utils.Helpers;
using Plugin.Connectivity;
using Plugin.Share;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class ConferenceInfoViewModel : ViewModelBase, INavigationAware
    {

        private IWiFiConfig _wifiConfig;
        private const string WifiUrl = "https://devopenspace.de/app/wlan/";
        public ConferenceInfoViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService, IWiFiConfig wifiConfig)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            _wifiConfig = wifiConfig;
        }

        private string _conduct = "We want Developer Open Space 2016 to be the best conference you’ve ever attended. In addition to great content, technical training, networking opportunities, and fun events, we want to make sure the conference is a safe and productive environment for all participants." +
            "\n\nAs such, we are dedicated to providing a harassment-free conference experience for everyone regardless of gender, sexual orientation, disability, physical appearance, body size, race, or religion. We do not tolerate harassment of conference participants in any form. Sexual language and imagery is not appropriate for any conference venue, including talks. Conference participants violating these rules may be asked to leave(without a refund) at the discretion of the conference organizers." +
            "\n\nHarassment includes offensive verbal comments related to gender, sexual orientation, disability, physical appearance, body size, race, religion, sexual images in public spaces, deliberate intimidation, stalking, following, harassing photography or recording, sustained disruption of talks or other events, inappropriate physical contact, and unwelcome sexual attention. Participants asked to stop any harassing behavior are expected to comply immediately." +
            "\n\nExhibitors in the expo hall, sponsor or vendor booths, or similar activities are also subject to this code of conduct. In particular, exhibitors should not use sexualized images, activities, or other material. Booth staff (including volunteers) should not use sexualized clothing/uniforms/costumes, or otherwise create a sexualized environment." +
            "\n\nIf you are being harassed, notice that someone else is being harassed, or have any other concerns, please contact a member of conference staff immediately. Conference staff can be identified by t-shirts and special badges." +
            "\n\nConference staff will be happy to help participants contact hotel/venue security or local law enforcement, provide escorts, or otherwise assist those experiencing harassment to feel safe for the duration of the conference." +
            "\n\nWe thank our attendees, speakers and exhibitors for their help in keeping the event welcoming, respectful, and friendly to all participants, so that we all can enjoy a great conference.";
        public string Conduct
        {
            get { return _conduct; }
            set { SetProperty(ref _conduct, value); }
        }

        public async Task<bool> UpdateConfigs()
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;
                try
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        using (var client = new HttpClient())
                        {
                            client.Timeout = TimeSpan.FromSeconds(5);
                            var json = await client.GetStringAsync(WifiUrl);
                            var root = JsonConvert.DeserializeObject<WiFiRoot>(json);
                            Settings.WiFiSSID = root.SSID;
                            Settings.WiFiPass = root.Password;
                        }
                    }
                }
                catch
                {
                }

                try
                {

                    if (_wifiConfig != null)
                        WiFiConfigured = _wifiConfig.IsConfigured(Settings.WiFiSSID);
                }
                catch (Exception ex)
                {
                    ex.Data["Method"] = "UpdateConfigs";
                    Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                    return false;
                }

                try
                {
                    var jsonUrl = await StoreManager.ApplicationDataStore.GetApplicationData(ApplicationDataConst.ApplicationDataCodeOfConduct);
                    if(!string.IsNullOrWhiteSpace(jsonUrl?.Value))
                        Conduct = jsonUrl.Value;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                }

            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }


        bool wiFiConfigured;
        public bool WiFiConfigured
        {
            get { return wiFiConfigured; }
            set { SetProperty(ref wiFiConfigured, value); }
        }


        ICommand configureWiFiCommand;
        public ICommand ConfigureWiFiCommand =>
            configureWiFiCommand ?? (configureWiFiCommand = DelegateCommand.FromAsyncHandler(ExecuteConfigureWiFiCommand));

        private async Task ExecuteConfigureWiFiCommand()
        {
            if (_wifiConfig == null)
                return;

            Logger.Log($"{DevopenspaceLoggerKeys.WiFiConfig}, Type, 2.4Ghz", Prism.Logging.Category.Info, Priority.None);

            if (!_wifiConfig.ConfigureWiFi(Settings.WiFiSSID, Settings.WiFiPass))
            {
                WiFiConfigured = false;
                await SendWiFiError();
            }
            else
            {
                WiFiConfigured = true;
            }
        }

        private async Task SendWiFiError()
        {
            await PageDialogService.DisplayAlertAsync("Wi-Fi Configuration", "Unable to configure WiFi, you may have to configure manually or try again.", "OK");
        }

        ICommand copyPasswordCommand;
        public ICommand CopyPasswordCommand =>
        copyPasswordCommand ?? (copyPasswordCommand = DelegateCommand<string>.FromAsyncHandler(async (t) => await ExecuteCopyPasswordAsync(t)));

        async Task ExecuteCopyPasswordAsync(string pass)
        {
            Logger.Log(DevopenspaceLoggerKeys.CopyPassword, Prism.Logging.Category.Info, Priority.None);
            await CrossShare.Current.SetClipboardText(pass, "Password");
            Toast.SendToast("Password Copied");
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            await UpdateConfigs();
        }
    }
}
