using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Converters
{
    public class AccountListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var accountItems = value as int? ?? 0;
            var rowHeight = parameter as int? ?? 44;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -accountItems + 1;
            return (accountItems * rowHeight) - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
