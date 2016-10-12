using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Views
{
    public partial class UWPRootPage : MasterDetailPage
    {
        public UWPRootPage()
        {
            InitializeComponent();
            this.AttachToolbarItems();
        }
    }
}
