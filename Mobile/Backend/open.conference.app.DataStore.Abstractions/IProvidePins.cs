using open.conference.app.DataStore.Abstractions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IProvidePins
    {
        ObservableRangeCollection<Pin> Pins { get; }
    }
}
