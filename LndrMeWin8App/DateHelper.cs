using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Resources;

namespace LndrMeWin8App
{
    public class DateHelper
    {

        public static string _(string key)
        {
            var rl = new ResourceLoader();
            return rl.GetString(key);
            //var context = new ResourceContext();
            //var resourceMap = ResourceManager.Current.MainResourceMap;
            //return resourceMap.GetValue(key, context).ToString();
        }

        // Shamelessly adapted from http://gaskell.org/rails-helper-distance_of_time_in_words-ported-to-c/

        public static string TimeAgoInWords(DateTime fromTime)
        {
            return DistanceOfTimeInWords(fromTime, DateTime.Now);
        }

        public static string TimeAgoInWords(DateTime fromTime, bool includeSeconds)
        {
            return DistanceOfTimeInWords(fromTime, DateTime.Now, includeSeconds);
        }

        public static string DistanceOfTimeInWords(DateTime fromTime, DateTime toTime)
        {
            return DistanceOfTimeInWords(fromTime, toTime, false);
        }

        public static string DistanceOfTimeInWords(DateTime fromTime, DateTime toTime, bool includeSeconds)
        {
            TimeSpan ts = (toTime - fromTime).Duration();
            int distanceInMinutes = (int)ts.TotalMinutes;
            int distanceInSeconds = (int)ts.TotalSeconds;

            string inWords = string.Empty;

            if (distanceInMinutes <= 1)
            {
                if (includeSeconds)
                {
                    if (InRange(0, 4, distanceInSeconds)) { inWords = _("LessThanFiveSeconds"); }
                    else if (InRange(5, 9, distanceInSeconds)) { inWords = _("LessThanTenSeconds"); }
                    else if (InRange(10, 19, distanceInSeconds)) { inWords = _("LessThanTwentySeconds"); }
                    else if (InRange(20, 39, distanceInSeconds)) { inWords = _("HalfAMinute"); }
                    else if (InRange(40, 59, distanceInSeconds)) { inWords = _("LessThanAMinute"); }
                    else { inWords = _("OneMinute"); }
                }
                else
                {
                    inWords = distanceInMinutes == 0 ? _("LessThanAMinute") : _("OneMinute");
                }
            }
            else
            {
                if (InRange(2, 44, distanceInMinutes)) { inWords = string.Format(_("MultipleMinutes"), distanceInMinutes); }
                else if (InRange(45, 89, distanceInMinutes)) { inWords = _("AboutOneHour"); }
                else if (InRange(90, 1439, distanceInMinutes))
                {
                    inWords = string.Format(_("AboutMultipleHours"), RoundedDistance(distanceInMinutes, 60));
                }
                else if (InRange(1440, 2879, distanceInMinutes)) { inWords = _("OneDay"); }
                else if (InRange(2880, 43199, distanceInMinutes)) { inWords = string.Format(_("MultipleDays"), RoundedDistance(distanceInMinutes, 1440)); }
                else if (InRange(43200, 86399, distanceInMinutes)) { inWords = _("AboutOneMonth"); }
                else if (InRange(86400, 525599, distanceInMinutes)) { inWords = string.Format(_("MultipleMonths"), RoundedDistance(distanceInMinutes, 43200)); }
                else if (InRange(525600, 1051199, distanceInMinutes)) { inWords = _("AboutOneYear"); }
                else { inWords = string.Format(_("MultipleYears"), RoundedDistance(distanceInMinutes, 525600)); }
            }

            return inWords;
        }

        private static int RoundedDistance(int value, double dividedBy)
        {
            decimal d = Convert.ToDecimal(value / dividedBy);
            if (d > 0)
            {
                return (int)decimal.Ceiling(d);
            }
            else
            {
                return (int)decimal.Floor(d);
            }

            // MidpointRounding.AwayFromZero is not supported in Windows Phone 7 or Silverlight
            //return (int)decimal.Round(Convert.ToDecimal(value / dividedBy), MidpointRounding.AwayFromZero);
        }

        private static bool InRange(int low, int high, int value)
        {
            return (value >= low && value <= high);
        }
    }
}
