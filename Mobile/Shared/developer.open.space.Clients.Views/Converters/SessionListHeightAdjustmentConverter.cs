using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Converters
{
    public class SessionListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tweetCount = value as int? ?? 0;
            var rowHeight = parameter as int? ?? 120;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -tweetCount + 1;
            return (tweetCount * rowHeight) - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
