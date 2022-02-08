using System;
using Craft.Utils;

namespace DMI.FD.Domain
{
    public static class Helpers
    {
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
    }
}
