using open.conference.app.DataStore.Abstractions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IProvideToolbarItems
    {
        ObservableRangeCollection<ToolbarItem> ToolBarItems { get; }
    }
}
