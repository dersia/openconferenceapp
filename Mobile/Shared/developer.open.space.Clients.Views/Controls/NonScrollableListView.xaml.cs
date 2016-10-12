using Xamarin.Forms;

namespace developer.open.space.Clients.Views.Controls
{
    public partial class NonScrollableListView : ListView
    {
        public NonScrollableListView()
            : base(ListViewCachingStrategy.RecycleElement)
        {
            InitializeComponent();
        }
    }
}
