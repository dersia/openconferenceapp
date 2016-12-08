using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class SponsorStore : BaseStore<Sponsor>, ISponsorStore
    {
        public SponsorStore() : base() { }
        public override string Identifier => "Sponsor";
    }
}
