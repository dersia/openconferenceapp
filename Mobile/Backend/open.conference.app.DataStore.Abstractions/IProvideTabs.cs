using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using open.conference.app.DataStore.Abstractions.Helpers;
using Xamarin.Forms;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IProvideTabs
    {
        ObservableRangeCollection<Page> Tabs { get; }
    }
}
