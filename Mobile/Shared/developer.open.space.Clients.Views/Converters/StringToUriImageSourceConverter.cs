using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Converters
{
    public class StringToUriImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageUrl = value?.ToString();
            return imageUrl != null ? new UriImageSource { Uri = new Uri(imageUrl), CachingEnabled = true, CacheValidity = TimeSpan.FromDays(3) } : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
