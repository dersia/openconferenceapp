using System;
using Xamarin.Forms;
using developer.open.space.Clients.ViewModels;
using System.Globalization;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System.Diagnostics;
using developer.open.space.Utils.Helpers.Extensions;

namespace developer.open.space.Clients.Converters
{
    class SessionTimeDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var session = value as Session;
                if(session == null)
                    return string.Empty;

                return Device.OS == TargetPlatform.iOS ? session.GetDisplayTime() : session.GetDisplayName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class WorkshopTimeDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var workshop = value as Workshop;
                if (workshop == null)
                    return string.Empty;

                return Device.OS == TargetPlatform.iOS ? workshop.GetDisplayTime() : workshop.GetDisplayName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class SessionDateDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var session = value as Session;
                if(session == null)
                    return string.Empty;
                
                return session.GetDisplayName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class WorkshopDateDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var workshop = value as Workshop;
                if (workshop == null)
                    return string.Empty;

                return workshop.GetDisplayName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class EventDateDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var featured = value as FeaturedEvent;
                if(featured == null)
                    return string.Empty;
                
                return featured.GetDisplayName();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        
    class EventTimeDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var featured = value as FeaturedEvent;
                if(featured == null)
                    return string.Empty;

                return featured.GetDisplayTime();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class EventDayNumberDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(!(value is DateTime))
                    return string.Empty;

                return ((DateTime)value).ToLocalTime().Day;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class EventDayDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(!(value is DateTime))
                    return string.Empty;

                return ((DateTime)value).ToLocalTime().DayOfWeek.ToString().Substring(0,3).ToUpperInvariant();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class EventColorDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                if(!(value is DateTime))
                    return Color.FromHex("753BE4");

                return DateTime.UtcNow > ((DateTime)value).ToUniversalTime() ? Color.FromHex("D3D2D2") : Color.FromHex("753BE4");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return Color.FromHex("753BE4");
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HumanizeDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is DateTime) 
                {
                    var date = ((DateTime)value);
                    if (date.Kind == DateTimeKind.Local) 
                    {
                        return date/*.Humanize (false)*/.ToString();
                    }

                    return date/*.Humanize()*/.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert: " + ex);
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

