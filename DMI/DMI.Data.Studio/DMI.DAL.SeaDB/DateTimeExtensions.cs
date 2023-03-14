using System;

namespace DMI.DAL.SeaDB
{
    public static class DateTimeExtensions
    {
        public static string AsString(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day} {hour}:{minute}:{second}";
        }

        public static string AsShortString(this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}";
        }
    }
}
