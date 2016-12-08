using Xamarin.Forms;

namespace open.conference.app.Clients.Views
{
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
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
