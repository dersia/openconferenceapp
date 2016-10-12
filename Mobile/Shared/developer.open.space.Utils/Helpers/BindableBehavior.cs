using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Utils.Helpers
{
    /// <summary>
    /// This is copied from https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/
    /// This should be replaced, as soon as this is included in Prism.
    /// See also: https://github.com/PrismLibrary/Prism/pull/715
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableBehavior<T> : Behavior<T> where T : BindableObject
    {
        public T AssociatedObject { get; private set; }

        protected override void OnAttachedTo(T visualElement)
        {
            base.OnAttachedTo(visualElement);

            AssociatedObject = visualElement;

            if (visualElement.BindingContext != null)
                BindingContext = visualElement.BindingContext;

            visualElement.BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnDetachingFrom(T view)
        {
            view.BindingContextChanged -= OnBindingContextChanged;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}
