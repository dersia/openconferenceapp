using developer.open.space.Clients.ViewModels.Interfaces;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.UWP
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
