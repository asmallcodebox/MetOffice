using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace MODataPointTests
{
    [Binding]
    public class CustomConversions
    {
        [StepArgumentTransformation(@"now")]
        public DateTime HoursAgoTransformation()
        {
            return DateTime.Now;
        }

        [StepArgumentTransformation(@"(\d+) hours? ago")]
        public DateTime HoursAgoTransformation(int hoursAgo)
        {
            return DateTime.Now.Subtract(TimeSpan.FromHours(hoursAgo));
        }

        [StepArgumentTransformation(@"(\d+) hours? later")]
        public DateTime HoursLaterTransformation(int hoursLater)
        {
            return DateTime.Now.Add(TimeSpan.FromHours(hoursLater));
        }

        [StepArgumentTransformation(@"(\d+) days? later")]
        public DateTime DaysLaterTransformation(int daysLater)
        {
            return DateTime.Now.Add(TimeSpan.FromDays(daysLater));
        }

        [StepArgumentTransformation(@"(\d+) days? ago")]
        public DateTime DaysAgoTransformation(int daysAgo)
        {
            return DateTime.Now.Subtract(TimeSpan.FromDays(daysAgo));
        }

        internal static string ToDateTimeQueryString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        internal static DateTime ToObservationHourUTC(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
            return DateTime.SpecifyKind(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0), dateTime.Kind);
        }

        internal static DateTime ToForecastHourUTC(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
            int hour = dateTime.Hour;
            int forecastHour = hour / 3 * 3;
            return DateTime.SpecifyKind(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, forecastHour, 0, 0), dateTime.Kind);
        }

        internal static DateTime ToForecastDayUTC(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
            return dateTime.Date;
        }

        internal static DateTime ConvertToDateTime(string dateTime)
        {
            return DateTime.Parse(dateTime, null, DateTimeStyles.RoundtripKind);
        }

        internal static DateTime ConvertToDate(string dateTime)
        {
            return ConvertToDateTime(dateTime).Date;
        }

        internal static DateTime ToLastHour(DateTime dateTime)
        {
            return DateTime.SpecifyKind(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0), dateTime.Kind);
        }
    }
}
