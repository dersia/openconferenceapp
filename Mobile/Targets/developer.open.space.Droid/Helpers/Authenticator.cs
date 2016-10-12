using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Azure;
using Prism.Logging;

namespace developer.open.space.Droid
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
                return await StoreManager.MobileService.LoginAsync(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity,
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