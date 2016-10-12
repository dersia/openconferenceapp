using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace developer.open.space.Clients.Views.Controls
{
    public partial class ImageGallery : ScrollView
    {
        public static readonly BindableProperty ItemsSourceProperty =
               BindableProperty.Create(nameof(ItemsSource),
                   typeof(IList),
                   typeof(ImageGallery),
                   default(IList),
                   BindingMode.TwoWay,
                   propertyChanging: (bindableObject, oldValue, newValue) => {
                       ((ImageGallery)bindableObject).ItemsSourceChanging();
                   },
                   propertyChanged: (bindableObject, oldValue, newValue) => {
                       ((ImageGallery)bindableObject).ItemsSourceChanged(bindableObject, (IList)oldValue, (IList)newValue);
                   }
               );
        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem),
                typeof(object),
                typeof(ImageGallery),
                null,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((ImageGallery)bindable).UpdateSelectedIndex();
                }
            );
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex),
                typeof(int),
                typeof(ImageGallery),
                0,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((ImageGallery)bindable).UpdateSelectedItem();
                }
            );

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get;
            set;
        }

        public IList<View> Children
        {
            get
            {
                return imageStack.Children;
            }
        }


        public ImageGallery()
        {
            InitializeComponent();
        }

        void ItemsSourceChanging()
        {
            if (ItemsSource == null)
                return;
        }

        private void ItemsSourceChanged(BindableObject bindable, IList oldValue, IList newValue)
        {
            if (ItemsSource == null)
                return;

            SetImages();
            var notifyCollection = newValue as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                notifyCollection.CollectionChanged += (sender, args) => {
                    if (args.NewItems != null)
                    {
                        foreach (var newItem in args.NewItems)
                        {
                            var view = (View)ItemTemplate.CreateContent();
                            var bindableObject = view as BindableObject;
                            if (bindableObject != null)
                                bindableObject.BindingContext = newItem;
                            imageStack.Children.Add(view);
                        }
                    }
                    if (args.OldItems != null)
                    {
                        // not supported
                    }
                };
            }			
        }

        private void SetImages()
        {
            imageStack.Children.Clear();
            foreach (var newItem in ItemsSource)
            {
                var view = (View)ItemTemplate.CreateContent();
                var bindableObject = view as BindableObject;
                if (bindableObject != null)
                    bindableObject.BindingContext = newItem;
                imageStack.Children.Add(view);
            }
        }

        private void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext)
                return;

            SelectedIndex = Children
                .Select(c => c.BindingContext)
                .ToList()
                .IndexOf(SelectedItem);
        }

        private void UpdateSelectedItem()
        {
            SelectedItem = SelectedIndex > -1 ? Children[SelectedIndex].BindingContext : null;
        }
    }
}
