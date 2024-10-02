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
            stationInformation2
        };

        var sensorLocation1 = new SensorLocation
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8352",
            GdbFromDate = now,
            GdbToDate = maxDate,
            StationidDMI = 7913
        };

        var sensorLocation2 = new SensorLocation
        {
            ObjectId = 2,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8353",
            GdbFromDate = now,
            GdbToDate = maxDate,
            StationidDMI = 7914
        };

        var sensorLocations = new List<SensorLocation>
        {
            sensorLocation1, 
            sensorLocation2
        };

        var elevationAngles1 = new ElevationAngles
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8354",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = sensorLocation1.GlobalId,
            SensorLocation = sensorLocation1,
            Angle_N = 1,
            Angle_NE = 2,
            Angle_E = 3,
            Angle_SE = 4,
            Angle_S = 5,
            Angle_SW = 6,
            Angle_W = 7,
            Angle_NW = 8,
            AngleIndex = 4
        };

        var elevationAngles2 = new ElevationAngles
        {
            ObjectId = 2,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8355",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = sensorLocation2.GlobalId,
            SensorLocation = sensorLocation2,
            Angle_N = 8,
            Angle_NE = 7,
            Angle_E = 6,
            Angle_SE = 5,
            Angle_S = 4,
            Angle_SW = 3,
            Angle_W = 2,
            Angle_NW = 1,
            AngleIndex = 5
        };

        var elevationAngles = new List<ElevationAngles>
        {
            elevationAngles1,
            elevationAngles2
        };

        var serviceVisitReport1 = new ServiceVisitReport
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8356",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = stationInformation1.GlobalId,
            StationInformation = stationInformation1,
        };

        var serviceVisitReport2 = new ServiceVisitReport
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8357",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = stationInformation2.GlobalId,
            StationInformation = stationInformation2,
        };

        var serviceVisitReports = new List<ServiceVisitReport>
        {
            serviceVisitReport1,
            serviceVisitReport2
        };

        var contactPerson1 = new ContactPerson
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8358",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = stationInformation1.GlobalId,
            StationInformation = stationInformation1,
            Name = "Anders And"
        };

        var contactPerson2 = new ContactPerson
        {
            ObjectId = 1,
            GlobalId = "bce3d933-064a-4439-b6f5-3326471a8359",
            GdbFromDate = now,
            GdbToDate = maxDate,
            ParentGuid = stationInformation2.GlobalId,
            StationInformation = stationInformation2,
            Name = "Joachim von And"
        };

        var contactPersons = new List<ContactPerson>
        {
            contactPerson1,
            contactPerson2
        };

        context.StationInformations.AddRange(stationInformations);
        context.SensorLocations.AddRange(sensorLocations);
        context.ElevationAngles.AddRange(elevationAngles);
        context.ServiceVisitReports.AddRange(serviceVisitReports);
        context.ContactPersons.AddRange(contactPersons);
        context.SaveChanges();
    }
}