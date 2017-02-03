using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.Clients.Converters
{
    public class SponsorsListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var SponsorCount = value as int? ?? 0;
            var rowHeight = parameter as int? ?? 100;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -SponsorCount + 1;
            return rowHeight - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
