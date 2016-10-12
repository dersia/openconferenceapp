using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.Clients.ViewModels.Models
{
    public class DeepLinkPage
    {
        public AppPage Page { get; set; }
        public string Id { get; set; }
    }
    public enum AppPage
    {
        Feed,
        Sessions,
        Events,
        Sponsors,
        Venue,
        FloorMap,
        ConferenceInfo,
        Settings,
        Session,
        Speaker,
        Sponsor,
        Login,
        Event,
        Notification,
        TweetImage,
        WiFi,
        CodeOfConduct,
        Filter,
        Information,
        Tweet,
        Evals,
        Workshops,
        Workshop
    }
}
