using System;
using Xamarin.Forms;
using System.Globalization;
using System.Diagnostics;
using developer.open.space.DataStore.Abstractions.DataObjects;

namespace developer.open.space.Clients.Converters
{
    /// <summary>
    /// Used to return a filled or empty image string
    /// </summary>
    class IsFilledIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>  
        (bool)value ? $"{parameter}_filled.png" : $"{parameter}_empty.png";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SpeakerGravatarImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null && value is Speaker)
                {
                    var speaker = (Speaker)value;
                    Uri imageUri = speaker.AvatarUri;
                    if(imageUri == null && !string.IsNullOrWhiteSpace(speaker.AvatarUrl))
                    {
                        imageUri = new Uri(speaker.AvatarUrl);
                    }
                    if(imageUri == null && speaker.PhotoUri != null)
                    {
                        imageUri = speaker.PhotoUri;
                    }
                    if(imageUri == null && !string.IsNullOrWhiteSpace(speaker.PhotoUrl))
                    {
                        imageUri = new Uri(speaker.PhotoUrl);
                    }
                    if(imageUri != null)
                    {
                        return ImageSource.FromUri(imageUri);
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine("Unable to convert image to URI: " + ex);
            }
            return ImageSource.FromFile("profile_generic_big.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Used to reaturn the speaker image with caching or default
    /// </summary>
    class SpeakerImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace((string)value))
                {
                    return new UriImageSource
                    {
                        Uri = new Uri((string)value),
                        CachingEnabled = true,
                        CacheValidity = TimeSpan.FromDays(3)
                    };
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Unable to convert image to URI: " + ex);
            }

            return ImageSource.FromFile("profile_generic_big.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

