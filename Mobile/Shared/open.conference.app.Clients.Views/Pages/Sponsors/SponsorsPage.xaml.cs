﻿using Xamarin.Forms;

namespace open.conference.app.Clients.Views
{
    public partial class SponsorsPage : ContentPage
    {
        public SponsorsPage()
        {
            InitializeComponent();
            this.AttachToolbarItems();
            ListViewSponsors.AttachEffects();
        }

        //there is an issue with NavigatedFrom is not called for "Hardware-Buttons"
        //see https://github.com/PrismLibrary/Prism/issues/634
        protected override bool OnBackButtonPressed()
        {
            this.NavigateBackHook();
            return base.OnBackButtonPressed();
        }
    }
}

