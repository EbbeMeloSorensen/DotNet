using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace DMI.Utils
{
    public static class Math
    {
        public static double Mean(
            IEnumerable<double> values)
        {
            return values.Sum() / values.Count();
        }

        public static double StandardDeviation(
            IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public static double Covariance(
            IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public static double Correlation(
            IEnumerable<double> xValues,
            IEnumerable<double> yValues)
        {
            var xMean = Mean(xValues);
            var yMean = Mean(yValues);
            var xyMean = Mean(xValues.Zip(yValues, (x, y) => x * y));
            var x2Mean = Mean(xValues.Select(x => x * x));
            var y2Mean = Mean(yValues.Select(y => y * y));

            return (xyMean - xMean * yMean) /
                (System.Math.Sqrt(x2Mean - System.Math.Pow(xMean, 2)) *
                 System.Math.Sqrt(y2Mean - System.Math.Pow(yMean, 2)));
        }
    }

    public static class LongExtensions
    {
        public static DateTime ConvertFromEpochInMillisecondsToDateTime(
            this long epochInMilliseconds)
        {
            return new DateTime(1970, 1, 1).Add(new TimeSpan(epochInMilliseconds * 10000));
        }

        public static DateTime ConvertFromEpochInMicrosecondsToDateTime(
            this long epochInMicroseconds)
        {
            return new DateTime(1970, 1, 1).Add(new TimeSpan(epochInMicroseconds * 10));
        }

        public static string ConvertFromEpochInMicrosecondsToDateString(
            this long? epoch)
        {
            if (epoch.HasValue)
            {
                return epoch.Value.ConvertFromEpochInMicrosecondsToDateTime().AsDateString();
            }

            return null;
        }

        public static string ConvertFromEpochInMicrosecondsToDateTimeString(
            this long? epoch,
            bool includeMilliseconds)
        {
            if (epoch.HasValue)
            {
                return epoch.Value.ConvertFromEpochInMicrosecondsToDateTime().AsDateTimeString(includeMilliseconds);
            }

            return null;
        }

        public static string AsRFC3339(
            this long epochInMicroseconds)
        {
            var dateTime = epochInMicroseconds.ConvertFromEpochInMicrosecondsToDateTime();

            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}T{hour}:{minute}:{second}Z";
        }
    }

    public static class DoubleExtensions
    {
        public static string AsString(
            this double? number)
        {
            return number.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0}", number) : "";
        }

        public static double TrimCoordinate(
            this double coordinate,
            int decimals)
        {
            return System.Math.Round(coordinate, decimals);
        }

        public static double? TrimCoordinate(
            this double? coordinate,
            int decimals)
        {
            if (coordinate.HasValue)
            {
                return System.Math.Round(coordinate.Value, decimals);
            }

            return null;
        }
    }

    public static class DateTimeExtensions
    {
        public static long AsEpochInMicroSeconds(
            this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
        }

        public static string AsDateTimeString(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            return result;
        }

        public static string AsDateString(
            this DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            return $"{year}-{month}-{day}";
        }

        public static string AsRFC3339(
            this DateTime dateTime,
            bool includeMilliseconds)
        {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString().PadLeft(2, '0');
            var day = dateTime.Day.ToString().PadLeft(2, '0');

            var hour = dateTime.Hour.ToString().PadLeft(2, '0');
            var minute = dateTime.Minute.ToString().PadLeft(2, '0');
            var second = dateTime.Second.ToString().PadLeft(2, '0');

            var result = $"{year}-{month}-{day}T{hour}:{minute}:{second}";

            if (includeMilliseconds)
            {
                var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

                result += $".{millisecond}";
            }

            result += "Z";

            return result;
        }
    }

    public static class StringExtensions
    {
        public static string PrefixWithZeroIf4CharactersLong(
            this string s)
        {
            if (s.Length == 4)
            {
                return "0" + s;
            }

            return s;
        }

        public static string FixCapitalization(
            this string text)
        {
            int? indexOfSlashCharacter = null;

            if (text.Contains("/"))
            {
                indexOfSlashCharacter = text.IndexOf("/");
                text = text.Replace('/', ' ');
            }

            IEnumerable<string> words = text.Split(' ');

            var result = words.Select(w =>
            {
                if (w.ToUpper() == "II")
                {
                    return "II";
                }

                var firstLetter = w.Substring(0, 1).ToUpper();
                var remainingLetters = w.Substring(1, w.Length - 1).ToLower();
                return firstLetter + remainingLetters;
            }).Aggregate((c, n) => $"{c} {n}");

            if (indexOfSlashCharacter.HasValue)
            {
                var sb = new StringBuilder(result);
                sb[indexOfSlashCharacter.Value] = '/';

                result = sb.ToString();
            }

            return result;
        }

        public static string ExtractStationId(
            this string text)
        {
            return text.Trim().Split(' ').First();
        }

        public static string AsNanoqStationId(
            this string stationId)
        {
            if (stationId.Length == 4)
            {
                stationId = stationId.PadLeft(5, '0');
            }

            if (stationId.Substring(0, 2) == "04")
            {
                return stationId + "00";
            }

            if (stationId.Substring(0, 2) == "05")
            {
                return stationId + "20";
            }

            if (stationId.Substring(0, 2) == "06")
            {
                if (stationId == "06051" ||
                    stationId == "06116")
                {
                    return stationId + "01";
                }

                return stationId + "00";
            }

            if (stationId.Substring(0, 1) == "2" ||
                stationId.Substring(0, 1) == "3")
            {
                return stationId + "50";
            }

            throw new ArgumentException("Apparently not a valid station id");
        }

        public static DateTime? ConvertFromRFC3339StringToDateTime(
            this string text)
        {
            if (text == null)
            {
                return new DateTime?();
            }

            var year = int.Parse(text.Substring(0, 4));
            var month = int.Parse(text.Substring(5, 2));
            var day = int.Parse(text.Substring(8, 2));
            var hour = int.Parse(text.Substring(11, 2));
            var minute = int.Parse(text.Substring(14, 2));
            var second = int.Parse(text.Substring(17, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        public static long? ConvertFromRFC3339StringToEpoch(
            this string text)
        {
            var asDateTime = text.ConvertFromRFC3339StringToDateTime();

            if (!asDateTime.HasValue)
            {
                return new long?();
            }

            return asDateTime.Value.AsEpochInMicroSeconds();
        }
    }
}
