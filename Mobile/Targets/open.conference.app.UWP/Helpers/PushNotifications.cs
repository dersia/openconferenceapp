using open.conference.app.Clients.ViewModels.Interfaces;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.UWP
{
    public class PushNotifications : IPushNotifications
    {
        private ILoggerFacade _logger;
        private bool _isRegistered = false;

        public PushNotifications(ILoggerFacade logger)
        {
            _logger = logger;
        }

        public bool IsRegistered
        {
            get
            {
                return _isRegistered;
            }
        }

        public void OpenSettings()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterForNotifications()
        {
            try
            {
                //var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                //await open.conference.app.DataStore.Azure.StoreManager.MobileService.GetPush().RegisterAsync(channel.Uri);
                //_isRegistered = true;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            return false;
        }
    }
}
