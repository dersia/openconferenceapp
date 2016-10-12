using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Converters
{
    public class TweetListHeightAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var initialHeight = 100;
            switch(Device.OS)
            {
                case TargetPlatform.Android:
                    initialHeight = 145;
                    break;
                case TargetPlatform.iOS:
                    initialHeight = 140;
                    break;
                case TargetPlatform.Windows:
                case TargetPlatform.WinPhone:
                    initialHeight = 155;
                    break;
                case TargetPlatform.Other:
                default:
                    break;
            }
            var tweetCount = value as int? ?? 0;
            var rowHeight = parameter as int? ?? initialHeight;
            var platformAdjustment = Device.OS != TargetPlatform.Android ? 1 : -tweetCount + 2;
            return (tweetCount * rowHeight) - platformAdjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
