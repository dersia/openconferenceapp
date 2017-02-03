using open.conference.app.DataStore.Abstractions.DataObjects;
using open.conference.app.DataStore.Abstractions.Helpers;
using open.conference.app.Utils.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace open.conference.app.Utils.Helpers.Extensions
{
    public static class SessionExtensions
    {
        public static AppLinkEntry GetAppLink(this Session session)
        {
            var url = $"https://devopenspace.de/#workshops";

            var entry = new AppLinkEntry
            {
                Title = session.Title,
                Description = session.Abstract,
                AppLinkUri = new Uri(url, UriKind.RelativeOrAbsolute),
                IsLinkActive = true
            };

            if (Device.OS == TargetPlatform.iOS)
                entry.Thumbnail = ImageSource.FromFile("Icon.png");

            entry.KeyValues.Add("contentType", "Session");
            entry.KeyValues.Add("appName", "Dev Open Space");
            entry.KeyValues.Add("companyName", "devopenspace");

            return entry;
        }

        public static string GetIndexName(this Session e)
        {
            if (!e.StartTime.HasValue || !e.EndTime.HasValue || e.StartTime.Value.IsTBA())
                return "To be announced";

            var start = e.StartTime.Value.ToLocalTime();


            var startString = start.ToString("t");
            var end = e.EndTime.Value.ToLocalTime();
            var endString = end.ToString("t");

            var day = start.DayOfWeek.ToString();
            var monthDay = start.ToString("M");
            return $"{day}, {monthDay}, {startString}–{endString}";
        }

        public static string GetSortName(this Session session)
        {

            if (!session.StartTime.HasValue || !session.EndTime.HasValue || session.StartTime.Value.IsTBA())
                return "To be announced";

            var start = session.StartTime.Value.ToLocalTime();
            var startString = start.ToString("t");

            if (DateTime.Today.Year == start.Year)
            {
                if (DateTime.Today.DayOfYear == start.DayOfYear)
                    return $"Today {startString}";

                if (DateTime.Today.DayOfYear + 1 == start.DayOfYear)
                    return $"Tomorrow {startString}";
            }
            var day = start.ToString("M");
            return $"{day}, {startString}";
        }

        public static string GetDisplayName(this Session session)
        {
            if (!session.StartTime.HasValue || !session.EndTime.HasValue || session.StartTime.Value.IsTBA())
                return "TBA";

            var start = session.StartTime.Value.ToLocalTime();
            var startString = start.ToString("t");
            var end = session.EndTime.Value.ToLocalTime();
            var endString = end.ToString("t");



            if (DateTime.Today.Year == start.Year)
            {
                if (DateTime.Today.DayOfYear == start.DayOfYear)
                    return $"Today {startString}–{endString}";

                if (DateTime.Today.DayOfYear + 1 == start.DayOfYear)
                    return $"Tomorrow {startString}–{endString}";
            }
            var day = start.ToString("M");
            return $"{day}, {startString}–{endString}";
        }


        public static string GetDisplayTime(this Session session)
        {
            if (!session.StartTime.HasValue || !session.EndTime.HasValue || session.StartTime.Value.IsTBA())
                return "TBA";
            var start = session.StartTime.Value.ToLocalTime();


            var startString = start.ToString("t");
            var end = session.EndTime.Value.ToLocalTime();
            var endString = end.ToString("t");
            return $"{startString}–{endString}";
        }


        
        
    }

    
}
