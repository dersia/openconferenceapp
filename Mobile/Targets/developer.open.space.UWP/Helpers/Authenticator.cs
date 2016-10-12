using Microsoft.WindowsAzure.MobileServices;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Azure;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.UWP
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
                return await StoreManager.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
                throw ex;
            }
        }
    }
}
