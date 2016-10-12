using developer.open.space.DataStore.Abstractions;
using Xamarin.Forms;

namespace developer.open.space.Clients.Views
{
    public partial class WorkshopsPage : ContentPage
    {
        public WorkshopsPage()
        {
            InitializeComponent();
            this.AttachToolbarItems();
        }

        //there is an issue with NavigatedFrom is not called for "Hardware-Buttons"
        //see https://github.com/PrismLibrary/Prism/issues/634
        protected override bool OnBackButtonPressed()
        {
            this.NavigateBackHook();
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            this.NavigateToTabHook();
            base.OnAppearing();
        }
    }
}

