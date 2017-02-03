using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.Utils.Helpers.Extensions
{
    public static class DateTimeExtenstions
    {
        public static bool IsTBA(this DateTime date)
        {

            if (date.ToLocalTime().Year == DateTime.MinValue.Year)
                return true;

            return false;
        }


        public static string GetSortName(this DateTime e)
        {


            var start = e.ToLocalTime();

            if (DateTime.Today.Year == start.Year)
            {
                if (DateTime.Today.DayOfYear == start.DayOfYear)
                    return $"Today";

                if (DateTime.Today.DayOfYear - 1 == start.DayOfYear)
                    return $"Yesterday";

                if (DateTime.Today.DayOfYear + 1 == start.DayOfYear)
                    return $"Tomorrow";
            }
            var monthDay = start.ToString("M");
            return $"{monthDay}";
        }

        public static DateTime GetStartDay(this DateTime date)
        {

            try
            {
                date = date.ToLocalTime();
                return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert" + ex);
            }

            try
            {
                date = date.ToLocalTime();
                return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to convert" + ex);
            }
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }
    }
}
