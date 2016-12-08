using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class EventStore : BaseStore<FeaturedEvent>, IEventStore
    {
        public EventStore() : base() { }
        public override string Identifier => "FeaturedEvent";
    }
}
