using Prism.Events;
using System;

namespace open.conference.app.DataStore.Abstractions.PubSubEvents
{
    public class ErrorEvent : PubSubEvent<Exception>
    {
    }
}
