using open.conference.app.DataStore.Abstractions;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace open.conference.app.Clients.Views
{
    public static class UiExtensions
    {
        const string _navigationServiceName = "UnityPageNavigationService";
        public static void AttachToolbarItems(this Page page)
        {
            page.BindingContextChanged += (sender, e) =>
            {
                if (page.BindingContext != null && page.BindingContext is IProvideToolbarItems)
                {
                    foreach (var toolbarItem in ((IProvideToolbarItems)page.BindingContext).ToolBarItems)
                    {
                        page.ToolbarItems.Add(toolbarItem);
                    }
                }
            };
        }

        public static void AttachTabs(this MultiPage<Page> page)
        {
            page.BindingContextChanged += (sender, e) =>
            {
                if (page.BindingContext != null && page.BindingContext is IProvideTabs)
                {
                    foreach (var tab in ((IProvideTabs)page.BindingContext).Tabs)
                    {
                        page.Children.Add(tab);
                    }
                }
            };
        }
        

        public static void MoveMap(this Page page)
        {
            if (page is IProvideMap)
            {
                page.BindingContextChanged += (sender, e) =>
                {
                    if (page.BindingContext != null && page.BindingContext is IMoveMap)
                    {
                        var map = ((IProvideMap)page).Map;
                        if(map != null)
                        {
                            map.MoveToRegion(((IMoveMap)page.BindingContext).MapSpan);
                        }
                    }
                };
            }
        }

        public static void AttachMapPins(this Page page)
        {
            if (page is IProvideMap)
            {
                page.BindingContextChanged += (sender, e) =>
                {
                    if (page.BindingContext != null && page.BindingContext is IProvidePins)
                    {
                        var map = ((IProvideMap)page).Map;
                        if (map != null)
                        {
                            foreach(var pin in ((IProvidePins)page.BindingContext).Pins)
                            {
                                map.Pins.Add(pin);
                            }
                        }
                    }
                };
            }
        }

        public static void AttachGestureRecognizers(this View view)
        {
            view.BindingContextChanged += (sender, e) =>
            {
                if (view.BindingContext != null && view.BindingContext is IProvideGestureRecognizers)
                {
                    foreach (var gestureRecognizer in ((IProvideGestureRecognizers)view.BindingContext).GestureRecognizers)
                    {
                        view.GestureRecognizers.Clear();
                        view.GestureRecognizers.Add(gestureRecognizer);
                    }
                }
            };
        }

        public static void NavigateBackHook(this Page page)
        {
            if (page.BindingContext != null && page.BindingContext is INavigationAware)
            {
                ((INavigationAware)page.BindingContext).OnNavigatedFrom(new NavigationParameters());
            }
        }

        public static void NavigateToTabHook(this Page page)
        {
            if (page.Parent != null && page.Parent is SimpleNavigationPage && page.Parent.Parent is MultiPage<Page>)
            {                
                if (page.BindingContext != null && page.BindingContext is INavigationAware)
                {
                    ((INavigationAware)page.BindingContext).OnNavigatedTo(new NavigationParameters() { ["Tab"] = 1 });
                }
            }
        }

        public static void AttachEffects(this View view)
        {
            view.BindingContextChanged += (sender, e) =>
            {
                if (view.BindingContext != null && view.BindingContext is IProvideEffects)
                {
                    foreach (var effectName in ((IProvideEffects)view.BindingContext).Effects)
                    {
                        var effect = Effect.Resolve(effectName);
                        view.Effects.Add(effect);
                    }
                }
            };
        }
    }
}
