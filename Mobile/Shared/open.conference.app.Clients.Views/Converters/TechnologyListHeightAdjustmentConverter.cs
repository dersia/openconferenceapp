using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.Clients.Converters
{
    public class TechnologyListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var technologyItems = value as int? ?? 0;
            var rowHeight = parameter as int? ?? 44;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -technologyItems + 1;
            return (technologyItems * rowHeight) - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
