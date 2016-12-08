using open.conference.app.DataStore.Abstractions.DataObjects;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions.PubSubEvents
{
    public class SpeakerSelectedEvent : PubSubEvent<Speaker>
    {
    }
}
