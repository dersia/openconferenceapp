using Prism.Events;
using System;

namespace developer.open.space.DataStore.Abstractions.PubSubEvents
{
    public class ErrorEvent : PubSubEvent<Exception>
    {
    }
}
