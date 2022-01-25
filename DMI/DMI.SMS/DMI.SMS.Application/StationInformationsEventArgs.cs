using System;
using System.Collections.Generic;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Application
{
    public class StationInformationsEventArgs : EventArgs
    {
        public readonly IEnumerable<StationInformation> StationInformations;

        public StationInformationsEventArgs(
            IEnumerable<StationInformation> stationInformations)
        {
            StationInformations = stationInformations;
        }
    }
}