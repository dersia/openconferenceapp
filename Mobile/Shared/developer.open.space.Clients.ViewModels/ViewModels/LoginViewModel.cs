using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.Clients.Views;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using developer.open.space.DataStore.Abstractions.Helpers;
using developer.open.space.Utils.Helpers;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Prism.Services;
using developer.open.space.DataStore.Abstractions.PubSubEvents;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private const string EMAIL_REGEX = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        
        public LoginViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService)
            : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            if (!Settings.FirstRun)
            {
                Title = "My Account";
                var cancel = new ToolbarItem
                {
                    Text = "Cancel",
                    Command = DelegateCommand.FromAsyncHandler(async () =>
                    {
                        if (IsBusy)
                            return;
                        await Finish();
                        Settings.FirstRun = false;
                    }).ObservesCanExecute((arg) => IsBusy)
                };
                if (Device.OS != TargetPlatform.iOS)
                    cancel.Icon = "toolbar_close.png";
               ToolBarItems.Add(cancel);                
            }
        }

        private int _borderThickness;
        public int BorderThickness
        {
            get { return _borderThickness; }
            set { SetProperty(ref _borderThickness, value); }
        }

        private ImageSource _placeholder = ImageSource.FromFile("profile_generic_big.png");
        public ImageSource Placeholder
        {
            get { return _placeholder; }
            set { SetProperty(ref _placeholder, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public ICommand LoginWithTwitterCommand => DelegateCommand.FromAsyncHandler(ExecuteLoginWithTwitterAsync).ObservesCanExecute((arg) => IsBusy);
        public ICommand TextChangedCommand => DelegateCommand.FromAsyncHandler(async () => {
            var isValid = (Regex.IsMatch(Email, EMAIL_REGEX, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            if (isValid)
            {
                BorderThickness = 3;
                Placeholder = ImageSource.FromUri(new Uri(Gravatar.GetURL(Email)));

            }
            else
            {
                BorderThickness = 0;
                Placeholder = ImageSource.FromFile("profile_generic_big.png");
            }
            await Task.FromResult(0);
        });

        private async Task ExecuteLoginWithTwitterAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Message = "Signing in...";

                AccountResponse result = null;

                if (result == null)
                    result = await SSOClient.LoginAsync(_email);

                if (result?.Success ?? false)
                {
                    Message = "Updating schedule...";
                    Settings.FirstName = result.User?.FirstName ?? string.Empty;
                    Settings.LastName = result.User?.LastName ?? string.Empty;
                    Settings.Email = (_email ?? result.User?.Email ?? string.Empty).ToLowerInvariant();

                    EventAggregator.GetEvent<LoggedInEvent>().Publish();
                    Logger.Log(DevopenspaceLoggerKeys.LoginSuccess, Prism.Logging.Category.Info, Priority.None);
                    try
                    {
                        await StoreManager.SyncAllAsync(true);
                        Settings.Current.LastSync = DateTime.UtcNow;
                        Settings.Current.HasSyncedData = true;
                    }
                    catch (Exception ex)
                    {
                        //if sync doesn't work don't worry it is alright we can recover later
                        Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                    }
                    await Finish();
                    Settings.FirstRun = false;
                }
                else
                {
                    Logger.Log($"{DevopenspaceLoggerKeys.LoginFailure}, Reason, {result.Error}", Prism.Logging.Category.Warn, Priority.Medium);
                    await PageDialogService.DisplayAlertAsync("Unable to Sign in", result.Error, "OK");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{DevopenspaceLoggerKeys.LoginFailure}, Reason, {ex?.Message ?? string.Empty}", Prism.Logging.Category.Exception, Priority.High);
                await PageDialogService.DisplayAlertAsync("Unable to Sign in", "The email or password provided is incorrect.", "OK");
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
            }
        }

        public ICommand SignupCommand => DelegateCommand.FromAsyncHandler(async () => await ExecuteSignupAsync()).ObservesCanExecute((arg) => IsBusy);

        async Task ExecuteSignupAsync()
        {
            Logger.Log(DevopenspaceLoggerKeys.Signup, Prism.Logging.Category.Info,Priority.None);
            await CrossShare.Current.OpenBrowser("https://devopenspaceworkshop2016.azurewebsites.net/account/register",
                new BrowserOptions
                {
                    ChromeShowTitle = true,
                    ChromeToolbarColor = new ShareColor
                    {
                        A = 255,
                        R = 118,
                        G = 53,
                        B = 235
                    },
                    UseSafariWebViewController = true
                });
        }

        public ICommand CancelCommand => DelegateCommand.FromAsyncHandler(ExecuteCancelAsync).ObservesCanExecute((arg) => IsBusy);

        async Task ExecuteCancelAsync()
        {
            Logger.Log(DevopenspaceLoggerKeys.LoginCancel, Prism.Logging.Category.Info, Priority.None);
            if (Settings.FirstRun)
            {
                try
                {
                    Message = "Updating schedule...";
                    IsBusy = true;
                    await StoreManager.SyncAllAsync(false);
                    Settings.Current.LastSync = DateTime.UtcNow;
                    Settings.Current.HasSyncedData = true;
                }
                catch (Exception ex)
                {
                    //if sync doesn't work don't worry it is alright we can recover later
                    Logger.Log(ex.Message, Prism.Logging.Category.Exception, Priority.High);
                }
                finally
                {
                    Message = string.Empty;
                    IsBusy = false;
                }
            }
            await Finish();
            Settings.FirstRun = false;
        }

        private async Task Finish()
        {
            await Navigation.GoBackAsync(new NavigationParameters() { ["ComingBack"] = 1 });
        }
    }
}
