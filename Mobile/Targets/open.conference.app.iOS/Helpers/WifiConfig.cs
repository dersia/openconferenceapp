using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using open.conference.app.Clients.ViewModels.Interfaces;
using Prism.Logging;

namespace open.conference.app.iOS
{
    public class WiFiConfig : IWiFiConfig
    {
        private ILoggerFacade _logger;
        public WiFiConfig(ILoggerFacade logger)
        {
            _logger = logger;
        }

        #region IWiFiConfig implementation

        public bool ConfigureWiFi(string ssid, string password)
        {
            return true;
        }

        public bool IsConfigured(string ssid)
        {
            return false;
        }

        public bool IsWiFiOn()
        {
            return true;
        }

        #endregion
    }
}

