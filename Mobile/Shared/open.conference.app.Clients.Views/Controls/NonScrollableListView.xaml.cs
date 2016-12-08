using Xamarin.Forms;

namespace open.conference.app.Clients.Views.Controls
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
