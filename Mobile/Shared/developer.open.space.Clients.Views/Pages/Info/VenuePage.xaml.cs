using System;
using developer.open.space.DataStore.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace developer.open.space.Clients.Views
{
    public partial class VenuePage : ContentPage, IProvideMap
    {
        public VenuePage()
        {
            InitializeComponent();
            this.AttachToolbarItems();
            this.AttachMapPins();
            this.MoveMap();
        }

        public Map Map
        {
            get
            {
                return this.DeveloperOpenSpaceMap;
            }
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

