using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.Clients.ViewModels.Interfaces
{
    public interface IWiFiConfig
    {
        bool ConfigureWiFi(string ssid, string password);
        bool IsConfigured(string ssid);
        bool IsWiFiOn();
    }
}
