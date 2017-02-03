using System;
using Xamarin.Forms.Platform.Android;
using Android.Support.Design.Widget;
using Android.Runtime;
using Xamarin.Forms;
using Android.Widget;
using FormsToolkit;
using Android.Views;
using open.conference.app.Droid;
using open.conference.app.Clients.ViewModels.Models;

[assembly: ExportRenderer (typeof(open.conference.app.Clients.Views.Controls.NavigationView), typeof(NavigationViewRenderer))]
namespace open.conference.app.Droid
{
    public class NavigationViewRenderer : ViewRenderer<open.conference.app.Clients.Views.Controls.NavigationView, NavigationView>
    {
        NavigationView navView;
        ImageView profileImage;
        TextView profileName;
        protected override void OnElementChanged(ElementChangedEventArgs<open.conference.app.Clients.Views.Controls.NavigationView> e)
        {
            
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;


            var view = Inflate(Forms.Context, Resource.Layout.nav_view, null);
            navView = view.JavaCast<NavigationView>();


            navView.NavigationItemSelected += NavView_NavigationItemSelected;

            open.conference.app.Utils.Helpers.Settings.Current.PropertyChanged += SettingsPropertyChanged;
            SetNativeControl(navView);

            var header = navView.GetHeaderView(0);
            profileImage = header.FindViewById<ImageView>(Resource.Id.profile_image);
            profileName = header.FindViewById<TextView>(Resource.Id.profile_name);

            profileImage.Click += (sender, e2) => NavigateToLogin();
            profileName.Click += (sender, e2) => NavigateToLogin();

            UpdateName();
            UpdateImage();

            navView.SetCheckedItem(Resource.Id.nav_feed);
        }

        void NavigateToLogin()
        {
            if (open.conference.app.Utils.Helpers.Settings.Current.IsLoggedIn)
                return;
        }

        void SettingsPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(open.conference.app.Utils.Helpers.Settings.Current.Email))
            {
                UpdateName();
                UpdateImage();
            }
        }

        void UpdateName()
        {
            profileName.Text = open.conference.app.Utils.Helpers.Settings.Current.UserDisplayName;
        }

        void UpdateImage()
        {
            Koush.UrlImageViewHelper.SetUrlDrawable (profileImage, open.conference.app.Utils.Helpers.Settings.Current.UserAvatar, Resource.Drawable.profile_generic);
        }

        public override void OnViewRemoved(Android.Views.View child)
        {
            base.OnViewRemoved(child);
            navView.NavigationItemSelected -= NavView_NavigationItemSelected;
            open.conference.app.Utils.Helpers.Settings.Current.PropertyChanged -= SettingsPropertyChanged;
        }

        IMenuItem previousItem;

        void NavView_NavigationItemSelected (object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {


            if (previousItem != null )
                previousItem.SetChecked(false);

            navView.SetCheckedItem(e.MenuItem.ItemId);

            previousItem = e.MenuItem;

            int id = 0;
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_feed:
                    id = (int)AppPage.Feed;
                    break;
                case Resource.Id.nav_sessions:
                    id = (int)AppPage.Sessions;
                    break;
                case Resource.Id.nav_workshops:
                    id = (int)AppPage.Workshops;
                    break;
                case Resource.Id.nav_events:
                    id = (int)AppPage.Events;
                    break;
                case Resource.Id.nav_sponsors:
                    id = (int)AppPage.Sponsors;
                    break;
                case Resource.Id.nav_venue:
                    id = (int)AppPage.Venue;
                    break;
                case Resource.Id.nav_floor_map:
                    id = (int)AppPage.FloorMap;
                    break;
                case Resource.Id.nav_conference_info:
                    id = (int)AppPage.ConferenceInfo;
                    break;
                case Resource.Id.nav_settings:
                    id = (int)AppPage.Settings;
                    break;
                case Resource.Id.nav_evals:
                    id = (int)AppPage.Evals;
                    break;
            }
        }

        protected override NavigationView CreateNativeControl()
        {
            throw new NotImplementedException();
        }
    }
}

