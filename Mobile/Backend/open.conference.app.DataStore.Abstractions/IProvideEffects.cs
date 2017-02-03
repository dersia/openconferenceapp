using open.conference.app.DataStore.Abstractions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IProvideEffects
    {
        ObservableRangeCollection<string> Effects { get; }
    }
}
