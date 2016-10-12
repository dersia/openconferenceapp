using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using developer.open.space.Clients.Views.Controls;
using developer.open.space.iOS;

[assembly:ExportRenderer(typeof(NonScrollableListView), typeof(NonScrollableListViewRenderer))]
[assembly:ExportRenderer(typeof(AlwaysScrollView), typeof(AlwaysScrollViewRenderer))]
namespace developer.open.space.iOS
{
    public class NonScrollableListViewRenderer : ListViewRenderer
    {
        public static void Initialize()
        {
            var test = DateTime.UtcNow;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
                Control.ScrollEnabled = false;
            
        }
    }

    public class AlwaysScrollViewRenderer : ScrollViewRenderer
    {
        public static void Initialize()
        {
            var test = DateTime.UtcNow;
        }
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            this.AlwaysBounceVertical = true;
        }
    }
}

