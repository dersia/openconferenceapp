using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace developer.open.space.server.backend.Models
{
    public class DevopenspaceNotifications
    {
        public static DevopenspaceNotifications Instance = new DevopenspaceNotifications();

        public NotificationHubClient Hub { get; }

        public DevopenspaceNotifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(ConfigurationManager.AppSettings["HubConnection"], ConfigurationManager.AppSettings["HubEndpiont"]);
        }
    }
}