using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.Clients.Converters
{
    public class AboutListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var aboutItems = value as int? ?? 0;
            var rowHeight = parameter as int? ?? 44;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -aboutItems + 1;
            return (aboutItems * rowHeight) - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
