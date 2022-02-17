using System;
using System.Collections.Generic;
using DMI.SMS.Domain.Entities;
using Xunit;

namespace DMI.SMS.IO.UnitTest
{
    public class DataIOHandlerTest
    {
        [Fact]
        public void Test1()
        {
            var stationInformations = new List<StationInformation>
            {
                new StationInformation
                {
                    StationName = "Botanisk Have",
                    StationIDDMI = 5735,
                    Stationtype = StationType.Pluvio,
                    AccessAddress = "Helvede",
                    Country = Country.Denmark,
                    Status = Status.Active,
                    DateFrom = new DateTime(1975, 7, 24),
                    Wgs_lat = 5.1,
                    Wgs_long = 6.2,
                    Hha = 4.3
                },
                new StationInformation
                {
                    StationName = "Nakskov",
                    StationIDDMI = 5736,
                    Stationtype = StationType.Synop,
                    AccessAddress = "Helvede",
                    Country = Country.Denmark,
                    Status = Status.Active,
                    DateFrom = new DateTime(1972, 3, 17),
                    Wgs_lat = 5.2,
                    Wgs_long = 6.3,
                    Hha = 4.4
                }
            };

            // Dette skrives uden carriage returns - hvorfor gør det det?
            // Hvad er forskellen til den der unit test?
            var dataIOHandler = new DataIOHandler();
            dataIOHandler.ExportDataToXML(stationInformations, @"C:\Temp\Helvede.xml");
        }
    }
}