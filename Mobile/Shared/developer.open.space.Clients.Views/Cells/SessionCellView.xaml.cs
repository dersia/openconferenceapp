using Xamarin.Forms;
using System.Windows.Input;
using developer.open.space.DataStore.Abstractions.DataObjects;

namespace developer.open.space.Clients.Views.Cells
{

    public partial class SessionCellView : ContentView
    {
        public SessionCellView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty FavoriteCommandProperty = 
            BindableProperty.Create(nameof(FavoriteCommand), typeof(ICommand), typeof(SessionCellView), default(ICommand));

        public ICommand FavoriteCommand
        {
            get { return GetValue(FavoriteCommandProperty) as Command; }
            set { SetValue(FavoriteCommandProperty, value); }
        }
    }
}

