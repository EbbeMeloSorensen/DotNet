using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Application
{
    public interface IUIDataProvider
    {
        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        void CreateStationInformation(
            StationInformation stationInformation,
            bool assignUniqueObjectId);

        int CountAllStationInformations();

        int CountStationInformations(
            Expression<Func<StationInformation, bool>> predicate);

        int CountStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates);

        StationInformation GetStationInformation(int id);

        IList<StationInformation> GetAllStationInformations();

        IList<StationInformation> FindStationInformations(
            Expression<Func<StationInformation, bool>> predicate);

        IList<StationInformation> FindStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates);

        void UpdateStationInformation(
            StationInformation stationInformation);

        void UpdateStationInformations(
            IList<StationInformation> people);

        void DeleteStationInformation(
            StationInformation stationInformation);

        void DeleteDeleteStationInformations(
            IList<StationInformation> stationInformation);

        void ExportData(
            string fileName,
            IList<Expression<Func<StationInformation, bool>>> predicates);

        void GenerateSQLScriptForTurningElevationAngles(
            string fileName);
        
        void ImportData(string fileName);

        List<Tuple<DateTime, DateTime>> ReadObservationIntervalsForStation(
            string directoryName,
            string searchPattern,
            double maxTolerableDifferenceBetweenTwoObservationsInDays);

        event EventHandler<StationInformationEventArgs> StationInformationCreated;
        event EventHandler<StationInformationsEventArgs> StationInformationsUpdated;
        event EventHandler<StationInformationsEventArgs> StationInformationsDeleted;
    }
}
