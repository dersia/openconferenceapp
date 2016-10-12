using developer.open.space.DataStore.Abstractions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.DataStore.Abstractions
{
    public interface IProvideToolbarItems
    {
        ObservableRangeCollection<ToolbarItem> ToolBarItems { get; }
    }
}
