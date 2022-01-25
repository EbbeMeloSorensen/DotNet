using System;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Application
{
    public class StationInformationEventArgs : EventArgs
    {
        public readonly StationInformation StationInformation;

        public StationInformationEventArgs(
            StationInformation stationInformation)
        {
            StationInformation = stationInformation;
        }
    }
}