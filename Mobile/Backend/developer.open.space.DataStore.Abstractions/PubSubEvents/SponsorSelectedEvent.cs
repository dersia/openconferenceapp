using developer.open.space.DataStore.Abstractions.DataObjects;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions.PubSubEvents
{
    public class SponsorSelectedEvent : PubSubEvent<Sponsor>
    {
    }
}
