using open.conference.app.Clients.ViewModels.Interfaces;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.UWP
{
    public class LaunchTwitter : ILaunchTwitter
    {
        private ILoggerFacade _logger;

        public LaunchTwitter(ILoggerFacade logger)
        {
            _logger = logger;
        }

        public async Task<bool> OpenStatus(string statusId)
        {
            try
            {
                var uriTwitterApp = new Uri($"twitter://status?id={statusId}");
                return await Windows.System.Launcher.LaunchUriAsync(uriTwitterApp);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            return false;
        }

        public async Task<bool> OpenUserName(string username)
        {
            try
            {
                var uriTwitterApp = new Uri($"twitter://user?screen_name={username}");
                return await Windows.System.Launcher.LaunchUriAsync(uriTwitterApp);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            return false;
        }
    }
}
