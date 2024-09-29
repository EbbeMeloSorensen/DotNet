using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore;

public static class Seeding
{
    public static void SeedDatabase(
        SMSDbContextBase context)
    {
        //var temp = context.StationInformations;

        var stationInformation1 = new StationInformation
        {
            //GlobalId = context.StationInformations.Ge
            StationIDDMI = 7913,
            StationName = "Bamse"
        };

        var stationInformations = new List<StationInformation>
        {
            stationInformation1
        };

        // It will fail as long as you haven' created global ids etc
        if (false)
        {
            context.StationInformations.AddRange(stationInformations);
            context.SaveChanges();
        }
    }
}