using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IMoveMap
    {
        MapSpan MapSpan { get; }
    }
}
