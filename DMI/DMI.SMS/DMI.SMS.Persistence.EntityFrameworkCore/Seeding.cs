using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore;

public static class Seeding
{
    public static void SeedDatabase(
        SMSDbContextBase context)
    {
        var now = DateTime.UtcNow;
        var maxDate = new DateTime(9999, 12, 31, 23, 59, 59);

        var stationInformation1 = new StationInformation
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8350",
            GdbFromDate = now,
            GdbToDate = maxDate,
            StationIDDMI = 7913,
            StationName = "Bamse"
        };

        var stationInformation2 = new StationInformation
        {
            ObjectId = 2,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8351",
            GdbFromDate = now,
            GdbToDate = maxDate,
            StationIDDMI = 7914,
            StationName = "Kylling"
        };

        var stationInformations = new List<StationInformation>
        {
            stationInformation1,
            stationInformation2};

        context.StationInformations.AddRange(stationInformations);
        context.SaveChanges();
    }
}