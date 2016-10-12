using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Azure.Stores
{
    public class SponsorStore : BaseStore<Sponsor>, ISponsorStore
    {
        public SponsorStore() : base() { }
        public override string Identifier => "Sponsor";
    }
}
