using Microsoft.WindowsAzure.MobileServices;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Azure;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace developer.open.space.iOS
{
    public class Authenticator : IAuthenticate
    {
        private ILoggerFacade _logger;

        public Authenticator(ILoggerFacade logger)
        {
            _logger = logger;
        }

        public async Task<MobileServiceUser> Authenticate()
        {
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                // Present on vc.
                return await StoreManager.MobileService.LoginAsync(vc,
                    MobileServiceAuthenticationProvider.Twitter);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
                throw ex;
            }
        }
    }
}
