using Xamarin.Forms;
using System.Windows.Input;
using open.conference.app.DataStore.Abstractions.DataObjects;

namespace open.conference.app.Clients.Views.Cells
{

    public partial class WorkshopCellView : ContentView
    {
        public WorkshopCellView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty FavoriteCommandProperty = 
            BindableProperty.Create(nameof(FavoriteCommand), typeof(ICommand), typeof(WorkshopCellView), default(ICommand));

        public ICommand FavoriteCommand
        {
            get { return GetValue(FavoriteCommandProperty) as Command; }
            set { SetValue(FavoriteCommandProperty, value); }
        }
    }
}

