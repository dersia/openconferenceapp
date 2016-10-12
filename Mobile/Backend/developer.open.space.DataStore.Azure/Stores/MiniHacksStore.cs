using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Azure.Stores
{
    public class MiniHacksStore : BaseStore<MiniHack>, IMiniHacksStore
    {
        public MiniHacksStore() : base() { }

        public override string Identifier => "MiniHacks";
    }
}
